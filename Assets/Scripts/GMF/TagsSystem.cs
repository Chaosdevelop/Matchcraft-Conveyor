public class TagsCollection<T, T2> where T2 : Tag<T>
{

}

public abstract class Tag<T>
{
    int Order;
    int Id;

    public virtual T Execute(T parameter)
    {
        return parameter;
    }
}

public class PlayerValues
{
    /*	int MaxHealth => TagsSystem.Calculated("MaxHealth");*/
    int BaseMaxHealth = 10;

    void ConfigSet(int configval)
    {
        BaseMaxHealth = configval;
    }

    public void ChangeMaxHealthADd(int delta)
    {
        BaseMaxHealth += delta;
    }

    public void ChangeMaxHealthMul(int delta)
    {
        BaseMaxHealth += delta;
    }
}