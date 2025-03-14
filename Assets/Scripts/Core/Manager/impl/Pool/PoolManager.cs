using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 抽屉（池子中的时间）对象
/// </summary>
public class PoolData
{
    //用来存储抽屉中的对象
    private Stack<GameObject> dataStack = new Stack<GameObject>();

    // 记录使用中的对象
    private List<GameObject> usedList = new List<GameObject>();

    //抽屉上限 用来限制抽屉中的对象数量
    private int maxNum;

    //抽屉根对象 用来管理布局
    private GameObject rootObj;


    public int Count => dataStack.Count;// 申明容器中是否有对象
    public int UsedCount => usedList.Count;// 使用的容器中是否有对象

    /// <summary>
    /// 用于判断是否需要创建新的对象 小于最大值时返回true 需要创建
    /// </summary>
    public bool NeedCreate => usedList.Count < maxNum;


    /// <summary>
    /// 初始化构造函数
    /// </summary>
    /// <param name="root">缓存池名</param>
    /// <param name="name">抽屉父对象名</param>
    public PoolData(GameObject root, string name, GameObject usedObj)
    {
        if (PoolManager.isOpenLayout)// 如果开启布局优化
        {
            // 创建抽屉
            rootObj = new GameObject(name);
            // 设置抽屉的父对象
            rootObj.transform.SetParent(root.transform);
        }

        // 将使用过的对象放入使用中的列表
        PushUsedList(usedObj);

        // 获取抽屉上限
        PoolObject poolObj = usedObj.GetComponent<PoolObject>();
        if (poolObj == null)
        {
            Debug.LogError("请为需要被缓存池管理的预制件挂载PoolObject组件 并 设置数量上限");
            return;
        }
        maxNum = poolObj.maxNum;
    }

    /// <summary>
    /// 从抽屉中取出一个物体
    /// </summary>
    /// <returns>想要的对象</returns>
    public GameObject Pop()
    {
        // 从抽屉中取出一个物体
        GameObject obj;


        if (dataStack.Count > 0)
        {
            obj = dataStack.Pop();
            usedList.Add(obj);
        }
        else
        {
            //从使用中的列表中取出一个物体（0为最早使用的物体）
            obj = usedList[0];
            //将取出的物体从使用中的列表中移除
            usedList.RemoveAt(0);
            //仍然是使用 重新放入使用中的列表 放到最后
            usedList.Add(obj);

        }

        // 激活物体
        obj.SetActive(true);
        if (PoolManager.isOpenLayout)
        {
            // 断开父子关系
            obj.transform.SetParent(null);
        }
            

        return obj;
    }


    /// <summary>
    /// 放入抽屉
    /// </summary>
    /// <param name="obj">放入抽屉的物体</param>
    public void Push(GameObject obj)
    {
        // 失活 放入抽屉
        obj.SetActive(false);
        //该项目需求
        obj.transform.SetParent(null);
        if (PoolManager.isOpenLayout)// 如果开启布局优化
        {
            obj.transform.SetParent(rootObj.transform);
        }
        // 通过栈记录
        dataStack.Push(obj);
        // 不在使用从usedList中移除
        usedList.Remove(obj);
    }


    /// <summary>
    /// 将使用过的对象放入使用中的列表
    /// </summary>
    /// <param name="obj"></param>
    public void PushUsedList(GameObject obj)
    {
        usedList.Add(obj);
    }
}


/// <summary>
/// 缓存池对象管理模块
/// </summary>
public class PoolManager : BaseManager<PoolManager>
{
    // 缓冲池 柜子中有很多个抽屉，每个抽屉存放一种类型的物体
    private Dictionary<string, PoolData> poolDic = new Dictionary<string, PoolData>();

    // 池子根对象
    private GameObject poolObj;

    //TODO: 记得检测是否开启布局优化(发布前关闭)
    //是否开启布局优化
    public static bool isOpenLayout = false;


    private PoolManager() { }


    /// <summary>
    /// 从缓冲池中获取对象
    /// </summary>
    /// <param name="name">抽屉名</param>
    /// <returns>从缓存池取出对象</returns>
    public GameObject GetObj(string name) //TODO: maxNum 最大值可设置
    {
        // 如果为空，就创建一个
        if (poolObj == null && isOpenLayout)// 如果开启布局优化
        {
            poolObj = new GameObject("Pool");
        }

        GameObject obj;

        #region 优化前代码

        /*
        // 如果缓冲池中有这个抽屉，并且抽屉中有物体
        if (poolDic.ContainsKey(name) && poolDic[name].Count > 0)
        {
            // 从抽屉中取出一个物体
            obj = poolDic[name].Pop();
        }
        //如果没有这个抽屉，或者抽屉中没有物体
        else
        {
            // 从资源中加载一个物体
            obj = GameObject.Instantiate(Resources.Load<GameObject>(name));
            // 避免实例出来的对象携带clone字样
            obj.name = name;
        }
        */

        #endregion

        #region 上限优化

        // 如果没有这个抽屉，或者抽屉中没有物体 且未达到最大值
        if (!poolDic.ContainsKey(name) ||
            (poolDic[name].Count == 0 && poolDic[name].NeedCreate))
        {
            // 从资源中加载一个物体
            obj = GameObject.Instantiate(Resources.Load<GameObject>(name));
            // 避免实例出来的对象携带clone字样
            obj.name = name;

            if (!poolDic.ContainsKey(name))
            {
                // 如果没有这个抽屉，就创建一个抽屉
                poolDic.Add(name, new PoolData(poolObj, name, obj));
            }
            else
            {
                // 加入使用中的列表
                poolDic[name].PushUsedList(obj);
            }
            
        }
        // 当抽屉没物体时，且达到最大值 或者 抽屉有物体
        else
        {
            obj = poolDic[name].Pop();
        }

        #endregion

        return obj;
    }

    /// <summary>
    /// 往缓冲池中存放对象
    /// </summary>
    /// <param name="name">抽屉名</param>
    /// <param name="obj">放入的对象</param>
    public void PushObj(GameObject obj)
    {
        #region 已经于实现&优化

        /*
        // 隐藏物体
        // 或者可以放到及其远的位置
        obj.SetActive(false);

        // 失活物体放入池子中
        obj.transform.SetParent(poolObj.transform);
        */

        // 如果没有这个抽屉，就创建一个抽屉
        //if (!poolDic.ContainsKey(obj.name))
        //    poolDic.Add(obj.name, new PoolData(poolObj, obj.name));

        #endregion

        // 放入抽屉
        poolDic[obj.name].Push(obj);
    }

    /// <summary>
    /// 用于清空缓冲池
    /// </summary>
    public void ClearPool()
    {
        poolDic.Clear();
        poolObj = null;
    }
}
