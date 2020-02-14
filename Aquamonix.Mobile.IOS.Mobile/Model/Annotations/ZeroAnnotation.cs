using CoreLocation;

namespace Aquamonix.Mobile.IOS.Model
{
    public class ZeroAnnotation : AnnotationBase
    {
        public ZeroAnnotation(string title, CLLocationCoordinate2D coord) : base(title, coord)
        {
        }
    }
}