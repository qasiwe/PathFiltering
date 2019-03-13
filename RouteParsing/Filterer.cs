namespace RouteParsing
{
    public interface Filterer <T>
    {
        T Filter(double minpeak,double multiplier,int timeDelay);
        
    }
}