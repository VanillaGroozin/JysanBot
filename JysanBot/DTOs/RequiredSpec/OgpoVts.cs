using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JysanBot.DTOs.RequiredSpec
{
    public class OgpoVts
    {
        public int Id { get; set; }
        public double CarTypeCoef { get; set; }
        //Легковые машины                                - 1
        //Автобусы до 16 пассажирских мест(включительно) - 2
        //Автобусы свыше 16 пассажирских мест            - 3
        //Грузовые                                       - 4
        //Троллейбусы, трамваи                           - 5
        //Мототранспорт                                  - 6
        //Прицепы, полуприцепы                           - 7
        
        public double CarRegionCoef { get; set; }
        //Алматинская область                            - 1.78
        //Южно-Казахстанская область                     - 1.01
        //Восточно-Казахстанская область                 - 1.96
        //Костанайская область                           - 1.95
        //Карагандинская область                         - 1.39
        //Северо-Казахстанская область                   - 1.33
        //Акмолинская область                            - 1.32
        //Павлодарская область                           - 1.63
        //Жамбылская область                             - 1.
        //Актюбинская область                            - 1.35
        //Западно-Казахстанская область                  - 1.17
        //Кызылординская область                         - 1.09
        //Атырауская область                             - 2.69
        //Мангистауская область                          - 1.15
        //Алматы                                         - 2.96
        //Астана                                         - 2.2
        //ТС снятые с учета для последующей регистрации  - 1.
        //Временный въезд                                - 4.4
        //Шымкент                                        - 1.01
        //Туркестанская область                          - 1.01

        public double CityRegionCoef { get; set; }

        public double CarYearCoef { get; set; }

        public double TotalPrice { get; set; }


        public double CalculateTotalPrice()
        {
            double totalPrice = 4797.5 * CarTypeCoef * CarRegionCoef * CityRegionCoef * CarYearCoef;
            TotalPrice = totalPrice;
            return totalPrice;
        }

        //Доделать
        public void SetCarYearCoef (int carYear)
        {
            if (carYear <2012) CarYearCoef = 1.09;
            else if (carYear >=2012) CarYearCoef = 2.26;
        }

        public void SetCityRegionCoef(string cityRegionCoef)
        {
            if (cityRegionCoef == "Да") CityRegionCoef = 2.09;
            else if (cityRegionCoef == "Нет") CityRegionCoef = 3.26;
        }

        public void SetCarTypeCoef (string carTypeCoef)
        {
            if (carTypeCoef == "Легковые машины") CarTypeCoef = 2.09;
            else if (carTypeCoef == "Автобусы до 16 пассажирских мест (включительно)") CarTypeCoef = 3.26;
            else if (carTypeCoef == "Автобусы свыше 16 пассажирских мест") CarTypeCoef = 3.45;
            else if (carTypeCoef == "Грузовые)") CarTypeCoef = 3.98;
            else if (carTypeCoef == "Троллейбусы, трамваи") CarTypeCoef = 2.33;
            else if (carTypeCoef == "Мототранспорт") CarTypeCoef = 1;
            else if (carTypeCoef == "Прицепы, полуприцепы") CarTypeCoef = 1;
        }

        public void SetCarRegionCoef(string carRegionCoef)
        {
            if (carRegionCoef == "Алматинская область") CarRegionCoef = 1.78;
            else if (carRegionCoef == "Южно-Казахстанская область") CarRegionCoef = 1.01;
            else if (carRegionCoef == "Восточно-Казахстанская область") CarRegionCoef = 1.96;
            else if (carRegionCoef == "Костанайская область") CarRegionCoef = 1.95;
            else if (carRegionCoef == "Карагандинская область") CarRegionCoef = 1.39;
            else if (carRegionCoef == "Северо-Казахстанская область") CarRegionCoef = 1.33;
            else if (carRegionCoef == "Акмолинская область") CarRegionCoef = 1.32;
            else if (carRegionCoef == "Павлодарская область") CarRegionCoef = 1.63;
            else if (carRegionCoef == "Жамбылская область") CarRegionCoef = 1;
            else if (carRegionCoef == "Актюбинская область") CarRegionCoef = 1.35;
            else if (carRegionCoef == "Западно-Казахстанская область") CarRegionCoef = 1.17;
            else if (carRegionCoef == "Кызылординская область") CarRegionCoef = 1.09;
            else if (carRegionCoef == "Атырауская область") CarRegionCoef = 2.69;
            else if (carRegionCoef == "Мангистауская область") CarRegionCoef = 1.15;
            else if (carRegionCoef == "Алматы") CarRegionCoef = 2.96;
            else if (carRegionCoef == "Астана") CarRegionCoef = 2.2;
            else if (carRegionCoef == "ТС снятые с учета для последующей регистрации") CarRegionCoef = 1;
            else if (carRegionCoef == "Временный въезд") CarRegionCoef = 4.4;
            else if (carRegionCoef == "Шымкент") CarRegionCoef = 1.01;
            else if (carRegionCoef == "Туркестанская область") CarRegionCoef = 1.01;
        }
    }
}
