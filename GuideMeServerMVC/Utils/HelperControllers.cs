namespace GuideMeServerMVC.Utils
{
    public class HelperControllers
    {
        public static Boolean VerificaUserLogado(ISession session)
        {
            int? logado = session.GetInt32("UserId");
            if (logado != null && logado >0)
                return true;
            else
                return false;
        }

        public static int GetUserLogadoID(ISession session)
        {
            int? logado = session.GetInt32("UserId");
            if (logado != null && logado > 0)
                return (int)logado;
            else
                return -1;
        }
    }
}
