using System;

namespace sms
{
    public class Sms
    {
        public string Phone { get; set; }
        public string Message { get; set; }
        public int Id {get;}
        public DateTime SmsDateTime {get;}
        public int Smstat{get;}
        public string Sca{get;}
        public int SaveType{get;}
        public int Priority{get;}
        public int SmsType{get;}

        public Sms() { }

        public Sms(int id, string phone, string message, DateTime smsDateTime, int smstat, string sca, int saveType, int priority, int smsType)
        {
            Id = id;
            Phone = phone;
            Message = message.Replace("\n", "");
            SmsDateTime = smsDateTime;
            Smstat = smstat;
            Sca = sca;
            SaveType = saveType;
            Priority = priority;
            SmsType = smsType;
        }
    }
}