using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace Aquamonix.Mobile.Lib.Services
{
    public struct ServerVersion : IComparable
    {
        private readonly string _versionString;
        private readonly List<int> _versionNumbers;
        private bool _isValid;
        private static ServerVersion _null = new ServerVersion("0");

        public bool IsValid { get { return _isValid; } }

        public bool IsNull { get { return this.Equals("0"); } }

        public static ServerVersion Null { get { return _null; } }

        private ServerVersion(string versionString)
        {
            if (versionString == null)
                throw new NullReferenceException("Version string cannot be null");

            if (String.IsNullOrEmpty(versionString))
                versionString = "0";

            _versionString = versionString.Trim();
            _versionNumbers = new List<int>();
            _isValid = false;

            this.ParseInternal();
        }

        public override int GetHashCode()
        {
            return _versionString.GetHashCode();
        }

        public override string ToString()
        {
            return _versionString;
        }

        public bool Equals(string obj)
        {
            return (this._versionString == obj);
        }

        public override bool Equals(object obj)
        {
            if (obj != null)
            {
                return this.Equals(obj.ToString());
            }

            return false;
        }

        public int CompareTo(object obj)
        {
            if (obj is ServerVersion)
                return this.CompareTo((ServerVersion)obj);
            else
                return this.CompareTo(obj == null ? "" : obj.ToString()); 
        }

        public int CompareTo(string obj)
        {
            return CompareTo(new ServerVersion(obj));
        }

        public int CompareTo(ServerVersion obj)
        {
            int output = 0;

            int min = Math.Min(this._versionNumbers.Count, obj._versionNumbers.Count);
            int max = Math.Max(this._versionNumbers.Count, obj._versionNumbers.Count);

            for (int n = 0; n < min; n++)
            {
                if (this._versionNumbers[n] > obj._versionNumbers[n])
                {
                    output = 1;
                    break;
                }
                else if (this._versionNumbers[n] < obj._versionNumbers[n])
                {
                    output = -1;
                    break;
                }
            }

            if (output == 0)
            {
                if (this._versionNumbers.Count != obj._versionNumbers.Count)
                {
                    if (this._versionNumbers.Count > obj._versionNumbers.Count)
                    {
                        for (int n=min; n<max; n++)
                        {
                            if (this._versionNumbers[n] > 0)
                            {
                                output = 1;
                                break;
                            }
                        }
                    }
                    else
                    {
                        for (int n = min; n < max; n++)
                        {
                            if (obj._versionNumbers[n] > 0)
                            {
                                output = -1;
                                break;
                            }
                        }
                    }
                }
            }

            return output;
        }

        public static ServerVersion Parse(string version) {
            return new ServerVersion(version);
        }


        public static bool operator ==(ServerVersion a, ServerVersion b)
        {
            return (a.CompareTo(b) == 0);
        }

        public static bool operator ==(ServerVersion a, string b)
        {
            return (a.CompareTo(b) == 0);
        }

        public static bool operator ==(string a, ServerVersion b)
        {
            return (b.CompareTo(a) == 0);
        }

        public static bool operator !=(ServerVersion a, ServerVersion b)
        {
            return (a.CompareTo(b) != 0);
        }

        public static bool operator !=(ServerVersion a, string b)
        {
            return (a.CompareTo(b) != 0);
        }

        public static bool operator !=(string a, ServerVersion b)
        {
            return (b.CompareTo(a) != 0);
        }

        public static bool operator >(ServerVersion a, ServerVersion b)
        {
            return (a.CompareTo(b) > 0);
        }

        public static bool operator >(ServerVersion a, string b)
        {
            return (a.CompareTo(b) > 0);
        }

        public static bool operator >(string a, ServerVersion b)
        {
            return (b.CompareTo(a) < 0);
        }

        public static bool operator >=(ServerVersion a, ServerVersion b)
        {
            return (a.CompareTo(b) >= 0);
        }

        public static bool operator >=(ServerVersion a, string b)
        {
            return (a.CompareTo(b) >= 0);
        }

        public static bool operator >=(string a, ServerVersion b)
        {
            return (b.CompareTo(a) <= 0);
        }

        public static bool operator <(ServerVersion a, ServerVersion b)
        {
            return (a.CompareTo(b) < 0);
        }

        public static bool operator <(ServerVersion a, string b)
        {
            return (a.CompareTo(b) < 0);
        }

        public static bool operator <(string a, ServerVersion b)
        {
            return (b.CompareTo(a) > 0);
        }

        public static bool operator <=(ServerVersion a, ServerVersion b)
        {
            return (a.CompareTo(b) <= 0);
        }

        public static bool operator <=(ServerVersion a, string b)
        {
            return (a.CompareTo(b) <= 0);
        }

        public static bool operator <=(string a, ServerVersion b)
        {
            return (b.CompareTo(a) >= 0);
        }


        private void ParseInternal()
        {
            _isValid = false;
            if (_versionString != null)
            {
                string[] parts = _versionString.Split('.');
                if (parts.Length > 0)
                {
                    _isValid = true; 

                    for (int n = 0; n < parts.Length; n++)
                    {
                        //each one must be a digit 
                        if (parts[n].All(Char.IsDigit))
                        {
                            _versionNumbers.Add(Int32.Parse(parts[n]));
                        }
                        else
                        {
                            _isValid = false;
                            break;
                        }
                    }
                }
            }
        }
    }
}





