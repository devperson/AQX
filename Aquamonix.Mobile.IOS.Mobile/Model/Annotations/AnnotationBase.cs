using CoreLocation;
using MapKit;

namespace Aquamonix.Mobile.IOS.Model
{
    public class AnnotationBase : MKAnnotation
    {
        readonly string title;
        CLLocationCoordinate2D coord;

        public AnnotationBase(string title, CLLocationCoordinate2D coord)
        {
            this.title = title;
            this.coord = coord;
        }

        public override string Title
        {
            get
            {
                return title;
            }
        }

        public override CLLocationCoordinate2D Coordinate
        {
            get
            {
                return coord;
            }
        }
    }
}