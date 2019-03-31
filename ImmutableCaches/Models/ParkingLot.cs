namespace ImmutableCaches.Models
{
    public class ParkingLot
    {
        public string LotCode { get; protected set; }
        public string LotName { get; protected set; }

        protected ParkingLot()
        {
        }

        public ParkingLot(string locationCode, string lotName)
        {
            // validations
            LotCode = locationCode;
            LotName = lotName;
        }
    }
}
