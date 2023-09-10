namespace GuideMeServerMVC.TO
{
    public class LoginResponseTO
    {
        public LoginResponseTO()
        {
            Token = string.Empty;
            responseMsg =
            new HttpResponseMessage()
            {
                StatusCode =
               System.Net.HttpStatusCode.Unauthorized
            };
        }

        public string Token { get; set; }
        public HttpResponseMessage responseMsg
        {
            get; set;
        }

    }
}
