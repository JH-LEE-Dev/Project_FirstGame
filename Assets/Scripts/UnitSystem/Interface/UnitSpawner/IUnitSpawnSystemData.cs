using System.Collections.Generic;

public interface IUnitSpawnSystemData
{
    Character character { get; }
    Earth player { get; }
    List<Enemy> enemies { get; }
}
