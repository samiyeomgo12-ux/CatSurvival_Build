using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class PoolManager : MonoBehaviour
{
    // [FIX] 표기 통일(prefabs) — 변수명은 인스펙터 연결 바뀌니 원하면 그대로 둬도 됨
    public GameObject[] prefebs;

    List<GameObject>[] pools;

    void Awake()
    {
        // [SAFETY] 널/길이 체크
        if (prefebs == null || prefebs.Length == 0)
        {
            Debug.LogError("[PoolManager] prefebs 배열이 비어 있습니다.");
            prefebs = new GameObject[0];
        }

        pools = new List<GameObject>[prefebs.Length];
        for (int index = 0; index < pools.Length; index++)
            pools[index] = new List<GameObject>();

        // [DEBUG] 시작 시 인덱스-프리팹 매핑 출력 (필요 없으면 지우기)
        for (int i = 0; i < prefebs.Length; i++)
        {
            var name = prefebs[i] ? prefebs[i].name : "(null)";
            /*Debug.Log($"[PoolManager] index {i} => {name}");*/
        }
    }

    public GameObject Get(int index)
    {
        // [SAFETY] 인덱스 범위 체크
        if (index < 0 || index >= prefebs.Length)
        {
           /* Debug.LogError($"[PoolManager] 잘못된 인덱스 요청: {index} (배열 크기 {prefebs.Length})");*/
            return null;
        }

        GameObject select = null;

        // [FAST PATH] 비활성 객체 재사용
        var list = pools[index];
        for (int i = 0; i < list.Count; i++)
        {
            var item = list[i];
            if (!item) continue; // [SAFETY] 삭제된 참조 대응
            if (!item.activeSelf)
            {
                select = item;
                select.SetActive(true);
                // [DEBUG] 어떤 프리팹이 나왔는지 확인
                // Debug.Log($"[PoolManager] REUSE index {index} => {select.name}");
                return select;
            }
        }

        // [SAFETY] 원본 프리팹 널 체크
        if (prefebs[index] == null)
        {
            Debug.LogError($"[PoolManager] prefebs[{index}] 가 null 입니다. 인스펙터 슬롯 확인!");
            return null;
        }

        // [INSTANTIATE] 새로 생성
        select = Instantiate(prefebs[index], transform);
        list.Add(select);

        // [DEBUG]
        // Debug.Log($"[PoolManager] INSTANTIATE index {index} => {select.name}");

        return select;
    }
}
