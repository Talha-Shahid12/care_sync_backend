﻿namespace CareSync.DTOs
{
    public class DoctorDto
    {
        public string Specialization { get; set; }
        public string HospitalName { get; set; }
        public decimal ConsultationFee { get; set; }
        public List<FreeDayDto> FreeHours { get; set; }
        public string UserId { get; set; }
    }


}


