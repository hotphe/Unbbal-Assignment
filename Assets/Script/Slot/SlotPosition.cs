using System.Collections.Generic;
using UnityEngine;

public class SlotPosition : MonoBehaviour
{
    [SerializeField] private List<Transform> _positions = new List<Transform>();

    public void SetHero(IList<Hero> heroes)
    {
        for(int i =0; i< _positions.Count; i++)
        {
            if (heroes[i].IsMoving)
            {
                heroes[i].transform.SetParent(_positions[i].transform);
                heroes[i].MoveHero(_positions[i].position);
            }
            else
            {
                heroes[i].transform.SetParent(_positions[i].transform);
                heroes[i].transform.localPosition = Vector3.zero;
                heroes[i].transform.localScale = Vector3.one;
            }
        }
    }

    public IList<Transform> GetTransforms()
    {
        return _positions;
    }
}
