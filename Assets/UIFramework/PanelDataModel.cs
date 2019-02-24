using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// UI面板信息数据类，与json中属性对应
/// </summary>
[System.Serializable]
public class PanelDataModel  {
    [System.NonSerialized]
    public PanelType panelType;//JsonUtility不能直接完成字符串到自定枚举类型的转换，所以使其不可序列化，用下面的字符串来替他完成，再在要添加入字典时再进行字符串向枚举类型的转换
    public string panelTypeStr="";
    public string path = "";

}
