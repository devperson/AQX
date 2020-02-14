using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Aquamonix.Mobile.Lib.Domain
{
    [DataContract]
    public class CurrentStepValue
    {
        [DataMember(Name = PropertyNames.FromAngle)]
        public ValueVisible FromAngle { get; set; }

        [DataMember(Name = PropertyNames.ToAngle)]
        public ValueVisible ToAngle { get; set; }

        [DataMember(Name = PropertyNames.Direction)]
        public ValueVisible Direction { get; set; }

        [DataMember(Name = PropertyNames.ApplicationAmount)]
        public ValueVisible ApplicationAmount { get; set; }

        [DataMember(Name = PropertyNames.Speed)]
        public ValueVisible Speed { get; set; }

        [DataMember(Name = PropertyNames.EndGun)]
        public ValueVisible EndGun { get; set; }

        [DataMember(Name = PropertyNames.Auxiliary1)]
        public ValueVisible Auxiliary1 { get; set; }

        [DataMember(Name = PropertyNames.Auxiliary2)]
        public ValueVisible Auxiliary2 { get; set; }

        [DataMember(Name = PropertyNames.DurationMinutes)]
        public ValueVisible DurationMinutes { get; set; }

        [DataMember(Name = PropertyNames.Wet)]
        public ValueVisible Wet { get; set; }

    }
}
