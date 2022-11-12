using Message_app_backend.Entities;
using System.Xml.Linq;

namespace Message_app_backend.Dto.Auth
{
    public class DeviceInfoClaim
    {
        public string DeviceName { get; set; }
        public string DeviceModel { get; set; }

        public override bool Equals(object obj)
        {
            if (obj is DeviceInfoClaim)
            {
                DeviceInfoClaim deviceInfoClaim = (DeviceInfoClaim)obj;
                if(DeviceName.Equals(deviceInfoClaim.DeviceName) && DeviceModel.Equals(deviceInfoClaim.DeviceModel))
                {
                    return true;
                }
            }
            return false;
        }

        /// <see cref="System.Object.GetHashCode"/>
        public override int GetHashCode()
        {
            return DeviceName.GetHashCode();
        }
    }
}
