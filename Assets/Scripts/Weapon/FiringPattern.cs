using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FiringPatternType {
    One,
    All,
    Even
}

public interface IFiringPattern {
    List<int> GetShotDistribution(int firePointCount, int projectileCount);
}

public class AllPerFirePointParttern : IFiringPattern {
    public List<int> GetShotDistribution(int firePointCount, int projectileCount) {
        List<int> result = new List<int>();

        for (int i = 0; i < firePointCount; i++) {
            result.Add(projectileCount);
        }
        return result;
    }
}

public class OnePerFirePointPattern : IFiringPattern {
    public List<int> GetShotDistribution(int firePointCount, int projectileCount) {
        List<int> result = new List<int>();

        for (int i = 0; i < firePointCount; i++) {
            if (projectileCount > 0) {
                result.Add(1);
                projectileCount--;
            } else {
                result.Add(0);
            }
        }
        return result;
    }
}

public class EvenDistributionPattern : IFiringPattern {
    public List<int> GetShotDistribution(int firePointCount, int projectileCount) {
        List<int> result = new List<int>();
        int baseCount = projectileCount / firePointCount;
        int remainder = projectileCount % firePointCount;

        for(int i = 0; i < firePointCount;i++) {
            int count = baseCount + (i < remainder ? 1 : 0);
            result.Add(count);
        }
        return result;
    }
}
