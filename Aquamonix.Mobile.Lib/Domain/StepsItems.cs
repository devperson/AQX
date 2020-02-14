using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Aquamonix.Mobile.Lib.Domain
{
    [DataContract]
    public class StepsItems
    {
        [DataMember(Name = PropertyNames.FromAngle)]
        public StepsVisibleValues FromAngle { get; set; }

        [DataMember(Name = PropertyNames.ToAngle)]
        public StepsVisibleValues ToAngle { get; set; }

        [DataMember(Name = PropertyNames.Direction)]
        public StepsVisibleValues Direction { get; set; }

        [DataMember(Name = PropertyNames.ApplicationAmount)]
        public StepsVisibleValues ApplicationAmount { get; set; }

        [DataMember(Name = PropertyNames.Speed)]
        public StepsVisibleValues Speed { get; set; }

        [DataMember(Name = PropertyNames.EndGun)]
        public StepsVisibleValues EndGun { get; set; }

        [DataMember(Name = PropertyNames.Auxiliary1)]
        public StepsVisibleValues Auxiliary1 { get; set; }

        [DataMember(Name = PropertyNames.Auxiliary2)]
        public StepsVisibleValues Auxiliary2 { get; set; }

        [DataMember(Name = PropertyNames.DurationMinutes)]
        public StepsVisibleValues DurationMinutes { get; set; }

        [DataMember(Name = PropertyNames.Wet)]
        public StepsVisibleValues Wet { get; set; }

        [DataMember(Name = PropertyNames.Name)]
        public string Name { get; set; }
    }
}
