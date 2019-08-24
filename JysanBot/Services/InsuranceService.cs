using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JysanBot.Database;
using JysanBot.DTOs;
using JysanBot.DTOs.RequiredSpec;
using Telegram.Bot.Types;

namespace JysanBot.Services
{
    public class InsuranceService
    {
        private readonly GenericRepository<DTOs.User> _users;
        private readonly GenericRepository<Deal> _deals;
        private readonly GenericRepository<OgpoVts> _ogpoVts;
        private readonly GenericRepository<DTP> _DTP;
        public string GenerateHelloMessage(
            string userName,
            int userId)
        {
            string responseMessage = string.Empty;

            var user = GetUserInfo(userId);
            if (user != null)
            {
                responseMessage += $"Здравствуйте {user.Name}, вас приветствует официальный Bot страховой компании Jýsan Garant! \n\n";
            }
            else
            {
                CreateUser(userName, userId);
                responseMessage += "Здравствуйте, вас приветствует официальный Bot страховой компании Jýsan Garant! \n\n";;
            }

            return responseMessage;
        }

        public DTOs.User GetUserInfo(int userId)
        {
            var user = _users.Read(userId);
            if (user != null)
            {
                return user;
            }
            return null;
        }

        public void UserUpdate(DTOs.User user)
        {
            _users.Update(user);
        }

        private void CreateUser(string userName, int userId, Contact phoneNum = null, string IIN = null)
        {
            _users.Add(new DTOs.User()
            {
                Id = userId,
                Name = userName,
                Contact = phoneNum,
                IIN = IIN,
                DTPs = new DTP()
            }); ;
        }

        public DTOs.DTP GetDtpInfo(int DTPId)
        {
            var DTP = _DTP.Read(DTPId);
            if (DTP != null)
            {
                return DTP;
            }
            return null;
        }

        public void DtpUpdate(DTOs.DTP DTP)
        {
            _DTP.Update(DTP);
        }


        public OgpoVts CreateOgpoVts(string carType, string carRegion, string cityRegion, int carYear)
        {
            OgpoVts ogpoVts = new OgpoVts();
            ogpoVts.SetCarTypeCoef(carType);
            ogpoVts.SetCarRegionCoef(carRegion);
            ogpoVts.SetCarYearCoef(carYear);
            ogpoVts.SetCityRegionCoef(cityRegion);
            ogpoVts.CalculateTotalPrice();
            _ogpoVts.Add(ogpoVts);
            return ogpoVts;
        }

        public void ShowOgpoList()
        {
            var ogpoVts = _ogpoVts.Read();
            foreach (var item in ogpoVts)
            {
                Console.WriteLine($"Total price: {item.TotalPrice}");
            }
        }



        public InsuranceService ()
        {
            _users = new GenericRepository<DTOs.User>();
            _DTP = new GenericRepository<DTOs.DTP>();
            _deals = new GenericRepository<Deal>();
            _ogpoVts = new GenericRepository<OgpoVts>();
        }
    }
}
