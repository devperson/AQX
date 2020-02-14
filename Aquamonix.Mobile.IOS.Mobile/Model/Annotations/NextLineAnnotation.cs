using CoreLocation;

namespace Aquamonix.Mobile.IOS.Model
{
    public class NextLineAnnotation : AnnotationBase
    {
        public NextLineAnnotation(string title, CLLocationCoordinate2D coord) : base(title, coord)
        {
        }
    }
}