namespace CGJ24.Classes;

public class Upgrade
{
    private int price;
    private int level;
    private int levelMax;
    
    public Upgrade(int price, int level, int levelMax)
    {
        this.price = price;
        this.level = level;
        this.levelMax = levelMax;
    }

    public bool CanUpgrade(int money)
    {
        if (price > money) return false;
        return (level >= levelMax);
    }

    public void Up()
    {
        level++;
    }

    public int Reset()
    {
        var temp = level * price;
        level = 0;
        return temp;
    }

    public int GetLevel()
    {
        return level;
    }
}