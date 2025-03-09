using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterSpawner : MonoBehaviour
{
    [SerializeField] private List<Transform> _points = new List<Transform>(4);

    private List<ResourceModel> _resourceModels = new List<ResourceModel>();
    private List<WaveModel> _waveModels = new List<WaveModel>();
    private List<Monster> _bossMonster = new List<Monster>();

    private bool _isOver = false;
    public int BossMonsterCount => _bossMonster.Count;
    
    public event Action OnSpawn;

    public void Init(ResourceModel resourceModel, WaveModel waveModel)
    {
        _resourceModels.Add(resourceModel);
        _waveModels.Add(waveModel);
    }

    public void Spawn(WaveInfo waveInfo)
    {
        if (waveInfo.Monster is BossMonster boss)
            SpawnBoss(waveInfo);
        else
            SpawnNormal(waveInfo).Forget();
    }

    public async UniTask SpawnNormal(WaveInfo waveInfo)
    {
        float _spwanInterval = waveInfo.Count / waveInfo.Time;
        for (int i =0; i < waveInfo.Count; i++)
        {
            var monster = Instantiate(waveInfo.Monster, _points[0].transform.position, Quaternion.identity);
            monster.transform.SetParent(transform);
            monster.Init(_points, waveInfo.Health);
            monster.OnDeath += GiveKillBonus;
            monster.Move();
            OnSpawn?.Invoke();
            await UniTask.WaitForSeconds(_spwanInterval).AttachExternalCancellation(destroyCancellationToken);

            if (_isOver)
                return;
        }
    }

    public void SetOver()
    {
        _isOver = true;
    }


    public void SpawnBoss(WaveInfo waveInfo)
    {
        var monster = Instantiate(waveInfo.Monster, _points[0].transform.position, Quaternion.identity);
        monster.transform.SetParent(transform);
        monster.Init(_points, waveInfo.Health);
        monster.OnDeath += GiveKillBonus;
        monster.OnDeath += BossDeath;
        monster.Move();
        _bossMonster.Add(monster);
        OnSpawn?.Invoke();
    }

    private void GiveKillBonus(Monster monster)
    {
        foreach (var model in _resourceModels)
        {
            model.AddCoin(monster.DropCoin);
            model.AddLuckyStone(monster.DropLuckyStone);
        }

        foreach(var model in _waveModels)
        {
            model.ReduceCurrentCount();
        }
    }

    private void BossDeath(Monster monster)
    {
        _bossMonster.Remove(monster);
    }
}
