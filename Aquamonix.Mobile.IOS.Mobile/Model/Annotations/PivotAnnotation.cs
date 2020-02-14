using CoreLocation;

namespace Aquamonix.Mobile.IOS.Model
{
    public class PivotAnnotation : AnnotationBase
    {
        public PivotAnnotation(string title, CLLocationCoordinate2D coord) : base(title, coord)
        {
        }
    }
}