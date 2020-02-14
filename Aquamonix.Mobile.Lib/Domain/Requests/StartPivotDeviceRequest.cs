using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Aquamonix.Mobile.Lib.ViewModels;

namespace Aquamonix.Mobile.Lib.Domain.Requests
{
    [DataContract]
    public class StartPivotDevice : ApiRequestforPivot<StartsPivotRequestBody>
    {
        public override bool IsProgressRequest
        {
            get { return true; }
        }

        public const string RequestKey = "Req.StartPivot";
        public StartPivotDevice(DeviceDetailViewModel model, string deviceId, Action onReconnect = null) : base(RequestKey)
        {
            this.Body = new StartsPivotRequestBody(model, deviceId);
        }
    }

    [DataContract]
    public class StartsPivotRequestBody
    {
        [DataMember(Name = PropertyNames.DeviceId)]
        public string DeviceId { get; set; }
        [DataMember(Name = PropertyNames.Programs)]
        public Programs Programs { get; private set; }


        public StartsPivotRequestBody(DeviceDetailViewModel model, string deviceId)
        {
            this.DeviceId = deviceId;
            var aux1 = new Auxiliary1
            {
                Value = model.Aux1Value
            };
            var aux2 = new Auxiliary2
            {
                Value = model.Aux2Value
            };
            var fromAngle = new FromAngle
            {
                Value = model.Device.CurrentStep == null ? "0" : model.Device.CurrentStep.FromAngle.Value == string.Empty ? "0" : model.Device.CurrentStep.FromAngle.Value
            };
            var toangle = new ToAngle
            {
                Value = model.ToAngleValue.ToString()
            };
            var applicationAmount = new ApplicationAmount
            {
                Value = model.AppAmountValue.ToString()
            };
            var endgun = new EndGun
            {
                Value = model.EndGunValue
            };
            var speed = new Speed
            {
                Value = model.Speed.ToString()
            };
            var direction = new Direction
            {
                Value = model.DirectionValue,
                //Visible = model.Device.CurrentStep.Direction == null ? "false" : model.Device.CurrentStep.Direction.Visible.ToString() 
           };
            var wet = new Wet
            {
                Value = model.WetDryValue
            };
            var durationMinutes = new DurationMinutes
            {
                Value = model.Device.CurrentStep == null ? "1" : model.Device.CurrentStep.DurationMinutes == null ? "1" : model.Device.CurrentStep.DurationMinutes.Value
            };
            var step1 = new Step
            {
                Auxiliary1 = aux1,
                DurationMinutes = durationMinutes
            };
            var step2 = new Step
            {
                Auxiliary1 = aux1,
                Auxiliary2 = aux2,
                FromAngle = fromAngle,
                ToAngle = toangle,
                Direction = direction,
                ApplicationAmount = applicationAmount,
                Speed = speed,
                EndGun = endgun,
                Wet = wet,
            };

            var continuous = new Continuous
            {
                Value = model.Continuous.ToString()
            };

            var autoReverse = new AutoReverse
            {
                Value= model.AutoReverseValue.ToString()
            };

            var itemSteps = new Steps();
            itemSteps.Add("0", step1);
            itemSteps.Add("1", step2);
            itemSteps.CurrentProgramId = null;

            var program = new Program
            {
                Continuous = continuous,
                AutoReverse = autoReverse,
                Steps = itemSteps
            };

            Programs = new Programs();
            Programs.Add("0", program);
            Programs.CurrentProgramId = null;
        }
    }

    [DataContract]
    public class Programs : ItemsDictionary<Program>
    {
       
    }

    [DataContract]
    public class Program
    {
        [DataMember(Name = PropertyNames.Continuous)]
        public Continuous Continuous { get; set; }

        [DataMember(Name = PropertyNames.AutoReverse)]
        public AutoReverse AutoReverse { get; set; }

        [DataMember(Name = PropertyNames.Steps)]
        public Steps Steps { get; set; }

        [DataMember(Name = PropertyNames.NumberOfRepeats)]
        public int NumberOfRepeats { get; set; }
    }

    [DataContract]
    public class Steps : ItemsDictionary<Step>
    {
    }

    [DataContract]
    public class Step
    {
        [DataMember(Name = PropertyNames.FromAngle)]
        public FromAngle FromAngle { get; set; }

        [DataMember(Name = PropertyNames.ToAngle)]
        public ToAngle ToAngle { get; set; }

        [DataMember(Name = PropertyNames.Direction)]
        public Direction Direction { get; set; }

        [DataMember(Name = PropertyNames.ApplicationAmount)]
        public ApplicationAmount ApplicationAmount { get; set; }

        [DataMember(Name = PropertyNames.Speed)]
        public Speed Speed { get; set; }

        [DataMember(Name = PropertyNames.EndGun)]
        public EndGun EndGun { get; set; }

        [DataMember(Name = PropertyNames.Auxiliary1)]
        public Auxiliary1 Auxiliary1 { get; set; }

        [DataMember(Name = PropertyNames.Auxiliary2)]
        public Auxiliary2 Auxiliary2 { get; set; }

        [DataMember(Name = PropertyNames.Wet)]
        public Wet Wet { get; set; }

        [DataMember(Name = PropertyNames.DurationMinutes)]
        public DurationMinutes DurationMinutes { get; set; }
    }

    [DataContract]
    public class Continuous
    {
        [DataMember(Name = PropertyNames.Value)]
        public string Value { get; set; }
    }

    [DataContract]
    public class AutoReverse
    {
        [DataMember(Name = PropertyNames.Value)]
        public string Value { get; set; }
    }

    [DataContract]
    public class FromAngle
    {
        [DataMember(Name = PropertyNames.Value)]
        public string Value { get; set; }
    }

    [DataContract]
    public class ToAngle
    {
        [DataMember(Name = PropertyNames.Value)]
        public string Value { get; set; }
    }

    [DataContract]
    public class Direction
    {
        [DataMember(Name = PropertyNames.Value)]
        public string Value { get; set; }
        [DataMember(Name = PropertyNames.Visible)]
        public string Visible { get; set; }
    }

    [DataContract]
    public class ApplicationAmount
    {
        [DataMember(Name = PropertyNames.Value)]
        public string Value { get; set; }
    }

    [DataContract]
    public class Speed
    {
        [DataMember(Name = PropertyNames.Value)]
        public string Value { get; set; }
    }

    [DataContract]
    public class EndGun
    {
        [DataMember(Name = PropertyNames.Value)]
        public string Value { get; set; }
    }

    [DataContract]
    public class Auxiliary2
    {
        [DataMember(Name = PropertyNames.Value)]
        public string Value { get; set; }
    }

    [DataContract]
    public class Wet
    {
        [DataMember(Name = PropertyNames.Value)]
        public string Value { get; set; }
    }

    [DataContract]
    public class Auxiliary1
    {
        [DataMember(Name = PropertyNames.Value)]
        public string Value { get; set; }
    }

    [DataContract]
    public class DurationMinutes
    {
        [DataMember(Name = PropertyNames.Value)]
        public string Value { get; set; }
    }
}

