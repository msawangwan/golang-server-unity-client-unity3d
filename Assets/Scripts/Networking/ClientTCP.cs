using UnityEngine;
using System;
using System.Text;
using System.Net;
using System.Net.Sockets;

/// <summary>
/// Asynchronous TCP Client.
/// </summary>

public enum ClientConnStatus {
    None = 0,
    Connected = 1,
    Disconnected = 2,
    Idle = 3,
    Timeout = 4,
}

public enum DataFrameStatus {
    None = 0,
    Normal = 1,
    KeepAlive = 2,
    Malformed = 3,
}

/* This is the argument passed on receiving an entire dataframe. */
public class RecvdDataFrameEventArgs {
    public DataFramePacket Data { get; private set; }
    public DataFrameStatus Status { get; private set; }

    public RecvdDataFrameEventArgs ( DataFramePacket dfp, DataFrameStatus dfs ) {
        Data = dfp;
        Status = dfs;
    }
}

/* Dataframe wrapper class. */
public class DataFramePacket {
    public byte[] Frame { get; private set; }

    /* Default constuctor. */
    public DataFramePacket ( int packetSize ) {
        Frame = new byte[packetSize];
    }
}

public class ClientTCP {
    /* Maintains socket state between socket reads. */
    sealed class SocketState {
        public Socket ReaderSocket { get; set; }
        public DataFramePacket DataFrame { get; set; }

        public byte[] FrameSize { get; set; }
        public int ExpectedFrameSize { get; set; }
        public int TotalBytesRead { get; set; }

        public bool IsFrameSizeKnown { get; set; }

        /* Default constuctor. */
        public SocketState () {
            FrameSize = new byte[4];

            DataFrame = null;

            ExpectedFrameSize = -1;
            TotalBytesRead = 0;

            IsFrameSizeKnown = false;
        }

        /* Helper method to reset state to defaults. */
        public void InitState() {
            FrameSize = new byte[4];

            ReaderSocket = null;
            DataFrame = null;

            ExpectedFrameSize = -1;
            TotalBytesRead = 0;

            IsFrameSizeKnown = false;
        }
    }

    /* Subscribers of this event will be notified when the client recvs a complete dataframe packet. */
    public Action<RecvdDataFrameEventArgs> RaiseDataFrameRecvd { get; set; }

    private const string serverAddr = "127.0.0.1";
    private const int serverPort = 9080;

    private static Socket serverSocket;
    private static IPAddress serverIP;
    private static IPEndPoint serverEndpoint;

    private ClientConnStatus connectionState = ClientConnStatus.None;

    /* Default constructor. */
    public ClientTCP ( ) {
        serverIP = IPAddress.Parse ( serverAddr );
        serverEndpoint = new IPEndPoint ( serverIP, serverPort );
    }

    public void Connect ( ) {
        serverSocket = OpenTCPSocket ( );
        serverSocket.BeginConnect ( serverEndpoint, new AsyncCallback ( OnConnect ), null );
    }

    public void Disconnect() {
        OnDisconnect ( );
    }

    public void SendAsync ( byte[] buffer ) {
        try {
            serverSocket.BeginSend ( buffer, 0, buffer.Length, 
                SocketFlags.None, new AsyncCallback ( OnSend ), null );
        } catch ( SocketException se ) {
            Debug.Log ( se );
        } catch ( Exception e ) {
            Debug.Log ( e );
        }
    }

    public void ListenAndRecvAsync ( ) {
        try {
            SocketState clientState = new SocketState();
            clientState.ReaderSocket = serverSocket;

            serverSocket.BeginReceive ( clientState.FrameSize, 0,
                clientState.FrameSize.Length,
                SocketFlags.None, new AsyncCallback ( OnRecv ),
                clientState ); // wait for incoming data
        } catch ( SocketException se ) {
            Debug.Log ( se );
        } catch ( Exception e ) {
            Debug.Log ( e );
        }
    }

    private void OnConnect ( IAsyncResult ar ) {
        try {
            serverSocket.EndConnect ( ar );
            connectionState = ClientConnStatus.Connected;
        } catch ( Exception e ) {
            Debug.Log ( e );
        }
    }

    private void OnDisconnect ( ) {
        try {
            serverSocket.Close ( );
            connectionState = ClientConnStatus.Disconnected;
        } catch ( Exception e ) {
            Debug.Log ( "[Client:OnDisconnect] Error on disconnect: " + e );
        }
    }

    private void OnSend ( IAsyncResult ar ) {
        try {
            serverSocket.EndSend ( ar );
        } catch ( Exception e ) {
            Debug.Log ( e );
        }
    }

    private void OnRecv ( IAsyncResult ar ) {
        SocketState socketState = ar.AsyncState as SocketState;
        try {
            int bytesRead = socketState.ReaderSocket.EndReceive(ar); // read bytes from stream
            socketState.TotalBytesRead += bytesRead; // keep track of how many bytes we've read from the stream for the current dataframe

            if ( socketState.ExpectedFrameSize == -1 ) { // get the data frame size
                if ( bytesRead <= 0 )
                    Debug.Log ( "Zero bytes recvd" ); // TODO: handle ...with exception?

                if ( socketState.TotalBytesRead == 4 ) { // we filled the 4-byte dataframe size buffer
                    int nbo = BitConverter.ToInt32(socketState.FrameSize, 0); // read the 4-byte buffer
                    socketState.ExpectedFrameSize = IPAddress.NetworkToHostOrder ( nbo ); // convert the read from network byte order to little e

                    if ( socketState.ExpectedFrameSize < 0 )
                        Debug.Log ( "Invalid Framesize" ); // TODO: handle ...                  

                    socketState.DataFrame = new DataFramePacket ( socketState.ExpectedFrameSize );
                    socketState.IsFrameSizeKnown = true;
                    socketState.TotalBytesRead = 0; // reset num bytes read cus we dont want the framesize byte count included
                }

                if ( socketState.ExpectedFrameSize != 0 ) { // if expectedframesize not 0 || -1, check why
                    if ( socketState.IsFrameSizeKnown ) { // if framesize known, switch to using the dataframe buffer
                        serverSocket.BeginReceive ( socketState.DataFrame.Frame, 0,
                            socketState.DataFrame.Frame.Length,
                            SocketFlags.None, new AsyncCallback ( OnRecv ),
                            socketState );
                    } else { // wait for more bytes to get the framesize
                        serverSocket.BeginReceive ( socketState.FrameSize, 
                            socketState.TotalBytesRead, // offset
                            socketState.FrameSize.Length - socketState.TotalBytesRead, // remaining bytes needed to get a complete framesize
                            SocketFlags.None, new AsyncCallback ( OnRecv ),
                            socketState );
                    }
                } else { // a 0 means keep alive
                    RecvdDataFrameEventArgs args = new RecvdDataFrameEventArgs(null, DataFrameStatus.KeepAlive);
                    RaiseDataFrameRecvd ( args ); // notify listeners of keep alive packet

                    socketState.InitState ( );
                }
            } else {
                if ( socketState.TotalBytesRead == socketState.ExpectedFrameSize ) { // we've got the entire msg
                    RecvdDataFrameEventArgs args = new RecvdDataFrameEventArgs(socketState.DataFrame, DataFrameStatus.Normal);
                    RaiseDataFrameRecvd ( args ); // notify listeners of the incoming daraframe

                    socketState.InitState ( ); // reset socket state
                    socketState.ReaderSocket = serverSocket;
                    serverSocket.BeginReceive ( socketState.FrameSize, 0, // reenter the listen loop
                        socketState.FrameSize.Length, SocketFlags.None, 
                        new AsyncCallback ( OnRecv ), socketState );
                } else {
                    if ( bytesRead <= 0 )
                        Debug.Log ( "Zero bytes recvd" ); // TODO: handle ...with exception?

                    serverSocket.BeginReceive ( socketState.DataFrame.Frame,
                        socketState.TotalBytesRead, // offset
                        socketState.DataFrame.Frame.Length - socketState.TotalBytesRead, // remaining buffer
                        SocketFlags.None, new AsyncCallback ( OnRecv ),
                        socketState );
                }
            }
        } catch ( ObjectDisposedException ode ) {
            Debug.Log ( ode );
        } catch ( Exception e ) {
            Debug.Log ( e );
        }
    }

    private static Socket OpenTCPSocket ( ) {
        return new Socket ( AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp );
    }
}