using System;

namespace Aquamonix.Mobile.IOS.Utilities.WebSockets
{
    public struct ConnectionState
    {
        private static ConnectionState _stateConnected = new ConnectionState("Connected");
        private static ConnectionState _stateConnecting = new ConnectionState("Connecting");
        private static ConnectionState _stateDisconnected = new ConnectionState("Disconnected");
        private static ConnectionState _stateDead = new ConnectionState("Dead");

        public static ConnectionState Connected { get { return _stateConnected; } }
        public static ConnectionState Connecting { get { return _stateConnecting; } }
        public static ConnectionState Disconnected { get { return _stateDisconnected; } }
        public static ConnectionState Dead { get { return _stateDead; } }

        public bool IsConnected { get { return this.Name == ConnectionState.Connected.Name; } }
        public bool IsConnecting { get { return this.Name == ConnectionState.Connecting.Name; } }
        public bool IsDisconnected { get { return this.Name == ConnectionState.Disconnected.Name; } }
        public bool IsDead { get { return this.Name == ConnectionState.Dead.Name; } }

        public string Name { get; private set; }
        public string Message { get; set; }

        public ConnectionState(string name)
        {
            Name = name;
            Message = null; 
        }

        public ConnectionState(string name, string message)
        {
            Name = name;
            Message = message;
        }

        public override string ToString()
        {
            return (String.IsNullOrEmpty(Message)) ? Name : String.Format("{0}: {1}", Name, Message);
        }

        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (Object.ReferenceEquals(obj, null) || !(obj is ConnectionState))
                return false;

            var o = (ConnectionState)obj;
            return (o.ToString().Equals(this.ToString()));
        }


        public static bool operator ==(ConnectionState a, ConnectionState b)
        {
            return (a.Equals(b));
        }

        public static bool operator !=(ConnectionState a, ConnectionState b)
        {
            return !(b.Equals(a));
        }

        public static bool operator ==(ConnectionState a, object b)
        {
            return !(a.Equals(b));
        }

        public static bool operator !=(ConnectionState a, object b)
        {
            return !(a.Equals(b));
        }

        public static bool operator ==(object a, ConnectionState b)
        {
            return (b.Equals(a));
        }

        public static bool operator !=(object a, ConnectionState b)
        {
            return !(b.Equals(a));
        }
    }
}
