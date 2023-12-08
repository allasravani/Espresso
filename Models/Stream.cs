using System.ComponentModel.DataAnnotations.Schema;

namespace ESPRESSO.Models
{
   
     public class UrlData
     {
        

        public int ID { get; set; }
        public required string Url { get; set; }
        public int COUNT { get; set; }
        public string? STREAM_NAME { get; set; }

        public string? EMAIL { get; set; }

        [Column(TypeName = "timestamp with time zone")]
        public DateTime LAST_UPDATED_DATE_AND_TIME{ get; set; }

}
    public class RequestData
    {
        public required string url { get; set; }
    }

    public class SmtpSettings
    {
        public required string Server { get; set; }
        public int Port { get; set; }
        public required string Username { get; set; }
        public required string Password { get; set; }
        public bool EnableSsl { get; set; }
    }
    public class EmailModel
    {
        public required string To { get; set; }
        public required string Subject { get; set; }
        public  string Body { get; set; }
    }
}
