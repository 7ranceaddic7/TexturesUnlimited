﻿using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace KSPShaderTools
{
    public static class Utils
    {

        #region parsing methods

        public static double safeParseDouble(String val)
        {
            double returnVal = 0;
            if (double.TryParse(val, out returnVal))
            {
                return returnVal;
            }
            return 0;
        }

        internal static bool safeParseBool(string v)
        {
            if (v == null) { return false; }
            else if (v.Equals("true") || v.Equals("yes") || v.Equals("1")) { return true; }
            return false;
        }

        public static float safeParseFloat(String val)
        {
            float returnVal = 0;
            try
            {
                returnVal = float.Parse(val);
            }
            catch (Exception e)
            {
                MonoBehaviour.print("ERROR: could not parse float value from: '" + val + "'\n" + e.Message);
            }
            return returnVal;
        }

        public static int safeParseInt(String val)
        {
            int returnVal = 0;
            try
            {
                returnVal = int.Parse(val);
            }
            catch (Exception e)
            {
                MonoBehaviour.print("ERROR: could not parse int value from: '" + val + "'\n" + e.Message);
            }
            return returnVal;
        }

        public static String[] parseCSV(String input)
        {
            return parseCSV(input, ",");
        }

        public static String[] parseCSV(String input, String split)
        {
            String[] vals = input.Split(new String[] { split }, StringSplitOptions.None);
            int len = vals.Length;
            for (int i = 0; i < len; i++)
            {
                vals[i] = vals[i].Trim();
            }
            return vals;
        }

        public static float[] parseFloatArray(string input)
        {
            string[] strs = parseCSV(input);
            int len = strs.Length;
            float[] flts = new float[len];
            for (int i = 0; i < len; i++)
            {
                flts[i] = safeParseFloat(strs[i]);
            }
            return flts;
        }

        public static Color parseColorFromBytes(string input)
        {
            Color color = new Color();
            float[] vals = parseFloatArray(input);
            color.r = vals[0] / 255f;
            color.g = vals[1] / 255f;
            color.b = vals[2] / 255f;
            if (vals.Length > 3)
            {
                color.a = vals[3] / 255f;
            }            
            return color;
        }

        public static Color parseColorFromFloats(string input)
        {
            input = input.Replace("(", "");
            input = input.Replace(")", "");
            Color color = new Color();
            float[] vals = parseFloatArray(input);
            color.r = vals[0];
            color.g = vals[1];
            color.b = vals[2];
            if (vals.Length > 3)
            {
                color.a = vals[3];
            }            
            return color;
        }

        public static ConfigNode parseConfigNode(String input)
        {
            ConfigNode baseCfn = ConfigNode.Parse(input);
            if (baseCfn == null) { MonoBehaviour.print("ERROR: Base config node was null!!\n" + input); }
            else if (baseCfn.nodes.Count <= 0) { MonoBehaviour.print("ERROR: Base config node has no nodes!!\n" + input); }
            return baseCfn.nodes[0];
        }

        #endregion

        #region ConfigNode extension methods

        public static String[] GetStringValues(this ConfigNode node, String name)
        {
            String[] values = node.GetValues(name);
            return values == null ? new String[0] : values;
        }

        public static string GetStringValue(this ConfigNode node, String name, String defaultValue)
        {
            String value = node.GetValue(name);
            return value == null ? defaultValue : value;
        }

        public static string GetStringValue(this ConfigNode node, String name)
        {
            return GetStringValue(node, name, "");
        }

        public static bool[] GetBoolValues(this ConfigNode node, String name)
        {
            String[] values = node.GetValues(name);
            int len = values.Length;
            bool[] vals = new bool[len];
            for (int i = 0; i < len; i++)
            {
                vals[i] = Utils.safeParseBool(values[i]);
            }
            return vals;
        }

        public static bool GetBoolValue(this ConfigNode node, String name, bool defaultValue)
        {
            String value = node.GetValue(name);
            if (value == null) { return defaultValue; }
            try
            {
                return bool.Parse(value);
            }
            catch (Exception e)
            {
                MonoBehaviour.print(e.Message);
            }
            return defaultValue;
        }

        public static bool GetBoolValue(this ConfigNode node, String name)
        {
            return GetBoolValue(node, name, false);
        }

        public static float[] GetFloatValues(this ConfigNode node, String name, float[] defaults)
        {
            String baseVal = node.GetStringValue(name);
            if (!String.IsNullOrEmpty(baseVal))
            {
                String[] split = baseVal.Split(new char[] { ',' });
                float[] vals = new float[split.Length];
                for (int i = 0; i < split.Length; i++) { vals[i] = Utils.safeParseFloat(split[i]); }
                return vals;
            }
            return defaults;
        }

        public static float[] GetFloatValues(this ConfigNode node, String name)
        {
            return GetFloatValues(node, name, new float[] { });
        }

        public static float[] GetFloatValuesCSV(this ConfigNode node, String name)
        {
            return GetFloatValuesCSV(node, name, new float[] { });
        }

        public static float[] GetFloatValuesCSV(this ConfigNode node, String name, float[] defaults)
        {
            float[] values = defaults;
            if (node.HasValue(name))
            {
                string strVal = node.GetStringValue(name);
                string[] splits = strVal.Split(',');
                values = new float[splits.Length];
                for (int i = 0; i < splits.Length; i++)
                {
                    values[i] = float.Parse(splits[i]);
                }
            }
            return values;
        }

        public static float GetFloatValue(this ConfigNode node, String name, float defaultValue)
        {
            String value = node.GetValue(name);
            if (value == null) { return defaultValue; }
            try
            {
                return float.Parse(value);
            }
            catch (Exception e)
            {
                MonoBehaviour.print(e.Message);
            }
            return defaultValue;
        }

        public static float GetFloatValue(this ConfigNode node, String name)
        {
            return GetFloatValue(node, name, 0);
        }

        public static double GetDoubleValue(this ConfigNode node, String name, double defaultValue)
        {
            String value = node.GetValue(name);
            if (value == null) { return defaultValue; }
            try
            {
                return double.Parse(value);
            }
            catch (Exception e)
            {
                MonoBehaviour.print(e.Message);
            }
            return defaultValue;
        }

        public static double GetDoubleValue(this ConfigNode node, String name)
        {
            return GetDoubleValue(node, name, 0);
        }

        public static int GetIntValue(this ConfigNode node, String name, int defaultValue)
        {
            String value = node.GetValue(name);
            if (value == null) { return defaultValue; }
            try
            {
                return int.Parse(value);
            }
            catch (Exception e)
            {
                MonoBehaviour.print(e.Message);
            }
            return defaultValue;
        }

        public static int GetIntValue(this ConfigNode node, String name)
        {
            return GetIntValue(node, name, 0);
        }

        public static int[] GetIntValues(this ConfigNode node, string name, int[] defaultValues = null)
        {
            int[] values = defaultValues;
            string[] stringValues = node.GetValues(name);
            if (stringValues == null || stringValues.Length == 0) { return values; }
            int len = stringValues.Length;
            values = new int[len];
            for (int i = 0; i < len; i++)
            {
                values[i] = Utils.safeParseInt(stringValues[i]);
            }
            return values;
        }

        public static Vector3 GetVector3(this ConfigNode node, String name, Vector3 defaultValue)
        {
            String value = node.GetValue(name);
            if (value == null)
            {
                return defaultValue;
            }
            String[] vals = value.Split(',');
            if (vals.Length < 3)
            {
                MonoBehaviour.print("ERROR parsing values for Vector3 from input: " + value + ". found less than 3 values, cannot create Vector3");
                return defaultValue;
            }
            return new Vector3((float)Utils.safeParseDouble(vals[0]), (float)Utils.safeParseDouble(vals[1]), (float)Utils.safeParseDouble(vals[2]));
        }

        public static Vector3 GetVector3(this ConfigNode node, String name)
        {
            String value = node.GetValue(name);
            if (value == null)
            {
                MonoBehaviour.print("ERROR: No value for name: " + name + " found in config node: " + node);
                return Vector3.zero;
            }
            String[] vals = value.Split(',');
            if (vals.Length < 3)
            {
                MonoBehaviour.print("ERROR parsing values for Vector3 from input: " + value + ". found less than 3 values, cannot create Vector3");
                return Vector3.zero;
            }
            return new Vector3((float)Utils.safeParseDouble(vals[0]), (float)Utils.safeParseDouble(vals[1]), (float)Utils.safeParseDouble(vals[2]));
        }

        public static FloatCurve GetFloatCurve(this ConfigNode node, String name, FloatCurve defaultValue = null)
        {
            FloatCurve curve = new FloatCurve();
            if (node.HasNode(name))
            {
                ConfigNode curveNode = node.GetNode(name);
                String[] values = curveNode.GetValues("key");
                int len = values.Length;
                String[] splitValue;
                float a, b, c, d;
                for (int i = 0; i < len; i++)
                {
                    splitValue = Regex.Replace(values[i], @"\s+", " ").Split(' ');
                    if (splitValue.Length > 2)
                    {
                        a = Utils.safeParseFloat(splitValue[0]);
                        b = Utils.safeParseFloat(splitValue[1]);
                        c = Utils.safeParseFloat(splitValue[2]);
                        d = Utils.safeParseFloat(splitValue[3]);
                        curve.Add(a, b, c, d);
                    }
                    else
                    {
                        a = Utils.safeParseFloat(splitValue[0]);
                        b = Utils.safeParseFloat(splitValue[1]);
                        curve.Add(a, b);
                    }
                }
            }
            else if (defaultValue != null)
            {
                foreach (Keyframe f in defaultValue.Curve.keys)
                {
                    curve.Add(f.time, f.value, f.inTangent, f.outTangent);
                }
            }
            else
            {
                curve.Add(0, 0);
                curve.Add(1, 1);
            }
            return curve;
        }

        public static Color GetColorFromFloatCSV(this ConfigNode node, String name)
        {
            return parseColorFromFloats(node.GetStringValue(name));
        }

        public static Color GetColorFromByteCSV(this ConfigNode node, String name)
        {
            return parseColorFromBytes(node.GetStringValue(name));
        }

        #endregion

        #region Transform extensionMethods

        /// <summary>
        /// Same as transform.FindChildren() but also searches for children with the (Clone) tag on the name.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="modelName"></param>
        /// <returns></returns>
        public static Transform[] FindModels(this Transform transform, String modelName)
        {
            Transform[] trs = transform.FindChildren(modelName);
            Transform[] trs2 = transform.FindChildren(modelName + "(Clone)");
            Transform[] trs3 = new Transform[trs.Length + trs2.Length];
            int index = 0;
            for (int i = 0; i < trs.Length; i++, index++)
            {
                trs3[index] = trs[i];
            }
            for (int i = 0; i < trs2.Length; i++, index++)
            {
                trs3[index] = trs2[i];
            }
            return trs3;
        }

        /// <summary>
        /// Same as transform.FindRecursive() but also searches for models with "(Clone)" added to the end of the transform name
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="modelName"></param>
        /// <returns></returns>
        public static Transform FindModel(this Transform transform, String modelName)
        {
            Transform tr = transform.FindRecursive(modelName);
            if (tr != null) { return tr; }
            return transform.FindRecursive(modelName + "(Clone)");
        }

        /// <summary>
        /// Same as transform.FindRecursive() but returns an array of all children with that name under the entire heirarchy of the model
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Transform[] FindChildren(this Transform transform, String name)
        {
            List<Transform> trs = new List<Transform>();
            if (transform.name == name) { trs.Add(transform); }
            locateTransformsRecursive(transform, name, trs);
            return trs.ToArray();
        }

        private static void locateTransformsRecursive(Transform tr, String name, List<Transform> output)
        {
            foreach (Transform child in tr)
            {
                if (child.name == name) { output.Add(child); }
                locateTransformsRecursive(child, name, output);
            }
        }

        /// <summary>
        /// Searches entire model heirarchy from the input transform to end of branches for transforms with the input transform name and returns the first match found, or null if none.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Transform FindRecursive(this Transform transform, String name)
        {
            if (transform.name == name) { return transform; }//was the original input transform
            Transform tr = transform.Find(name);//found as a direct child
            if (tr != null) { return tr; }
            foreach (Transform child in transform)
            {
                tr = child.FindRecursive(name);
                if (tr != null) { return tr; }
            }
            return null;
        }

        /// <summary>
        /// Uses transform.FindRecursive to search for the given transform as a child of the input transform; if it does not exist, it creates a new transform and nests it to the input transform (0,0,0 local position and scale).
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Transform FindOrCreate(this Transform transform, String name)
        {
            Transform newTr = transform.FindRecursive(name);
            if (newTr != null)
            {
                return newTr;
            }
            GameObject newGO = new GameObject(name);
            newGO.SetActive(true);
            newGO.name = newGO.transform.name = name;
            newGO.transform.NestToParent(transform);
            return newGO.transform;
        }

        /// <summary>
        /// Returns -ALL- children/grand-children/etc transforms of the input; everything in the heirarchy.
        /// </summary>
        /// <param name="transform"></param>
        /// <returns></returns>
        public static Transform[] GetAllChildren(this Transform transform)
        {
            List<Transform> trs = new List<Transform>();
            recurseAddChildren(transform, trs);
            return trs.ToArray();
        }

        private static void recurseAddChildren(Transform transform, List<Transform> trs)
        {
            int len = transform.childCount;
            foreach (Transform child in transform)
            {
                trs.Add(child);
                recurseAddChildren(child, trs);
            }
        }

        /// <summary>
        /// Returns true if the input 'isParent' transform exists anywhere upwards of the input transform in the heirarchy.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="isParent"></param>
        /// <param name="checkUpwards"></param>
        /// <returns></returns>
        public static bool isParent(this Transform transform, Transform isParent, bool checkUpwards = true)
        {
            if (isParent == null) { return false; }
            if (isParent == transform.parent) { return true; }
            if (checkUpwards)
            {
                Transform p = transform.parent;
                if (p == null) { return false; }
                else { p = p.parent; }
                while (p != null)
                {
                    if (p == isParent) { return true; }
                    p = p.parent;
                }
            }
            return false;
        }

        public static void recursePrintComponents(GameObject go, String prefix)
        {
            int childCount = go.transform.childCount;
            Component[] comps = go.GetComponents<Component>();
            MonoBehaviour.print("Found gameObject: " + prefix + go.name + " enabled: " + go.activeSelf + " inHierarchy: " + go.activeInHierarchy + " layer: " + go.layer + " children: " + childCount + " components: " + comps.Length + " position: " + go.transform.position + " scale: " + go.transform.localScale);
            foreach (Component comp in comps)
            {
                if (comp is MeshRenderer)
                {
                    MeshRenderer r = (MeshRenderer)comp;
                    Material m = r.material;
                    Shader s = m == null ? null : m.shader;
                    MonoBehaviour.print("Found Mesh Renderer component.  Mat/shader: " + m + " : " + s);
                }
                else
                {
                    MonoBehaviour.print("Found Component : " + prefix + "* " + comp);
                }
            }
            Transform t = go.transform;
            foreach (Transform child in t)
            {
                recursePrintComponents(child.gameObject, prefix + "  ");
            }
        }

        #endregion

        #region PartModule extensionMethods

        public static void updateUIFloatEditControl(this PartModule module, string fieldName, float min, float max, float incLarge, float incSmall, float incSlide, bool forceUpdate, float forceVal)
        {
            UI_FloatEdit widget = null;
            if (HighLogic.LoadedSceneIsEditor)
            {
                widget = (UI_FloatEdit)module.Fields[fieldName].uiControlEditor;
            }
            else if (HighLogic.LoadedSceneIsFlight)
            {
                widget = (UI_FloatEdit)module.Fields[fieldName].uiControlFlight;
            }
            else
            {
                return;
            }
            if (widget == null)
            {
                return;
            }
            widget.minValue = min;
            widget.maxValue = max;
            widget.incrementLarge = incLarge;
            widget.incrementSmall = incSmall;
            widget.incrementSlide = incSlide;
            if (forceUpdate && widget.partActionItem != null)
            {
                UIPartActionFloatEdit ctr = (UIPartActionFloatEdit)widget.partActionItem;
                var t = widget.onFieldChanged;//temporarily remove the callback
                widget.onFieldChanged = null;
                ctr.incSmall.onToggle.RemoveAllListeners();
                ctr.incLarge.onToggle.RemoveAllListeners();
                ctr.decSmall.onToggle.RemoveAllListeners();
                ctr.decLarge.onToggle.RemoveAllListeners();
                ctr.slider.onValueChanged.RemoveAllListeners();
                ctr.Setup(ctr.Window, module.part, module, HighLogic.LoadedSceneIsEditor ? UI_Scene.Editor : UI_Scene.Flight, widget, module.Fields[fieldName]);
                widget.onFieldChanged = t;//re-seat callback
            }
        }

        public static void updateUIFloatEditControl(this PartModule module, string fieldName, float newValue)
        {
            UI_FloatEdit widget = null;
            if (HighLogic.LoadedSceneIsEditor)
            {
                widget = (UI_FloatEdit)module.Fields[fieldName].uiControlEditor;
            }
            else if (HighLogic.LoadedSceneIsFlight)
            {
                widget = (UI_FloatEdit)module.Fields[fieldName].uiControlFlight;
            }
            else
            {
                return;
            }
            if (widget == null)
            {
                return;
            }
            BaseField field = module.Fields[fieldName];
            field.SetValue(newValue, field.host);
            if (widget.partActionItem != null)//force widget re-setup for changed values; this will update the GUI value and slider positions/internal cached data
            {
                UIPartActionFloatEdit ctr = (UIPartActionFloatEdit)widget.partActionItem;
                var t = widget.onFieldChanged;//temporarily remove the callback; we don't need an event fired when -we- are the ones editing the value...            
                widget.onFieldChanged = null;
                ctr.incSmall.onToggle.RemoveAllListeners();
                ctr.incLarge.onToggle.RemoveAllListeners();
                ctr.decSmall.onToggle.RemoveAllListeners();
                ctr.decLarge.onToggle.RemoveAllListeners();
                ctr.slider.onValueChanged.RemoveAllListeners();
                ctr.Setup(ctr.Window, module.part, module, HighLogic.LoadedSceneIsEditor ? UI_Scene.Editor : UI_Scene.Flight, widget, module.Fields[fieldName]);
                widget.onFieldChanged = t;//re-seat callback
            }
        }

        public static void updateUIChooseOptionControl(this PartModule module, string fieldName, string[] options, string[] display, bool forceUpdate, string forceVal = "")
        {
            if (display.Length == 0 && options.Length > 0) { display = new string[] { "NONE" }; }
            if (options.Length == 0) { options = new string[] { "NONE" }; }
            UI_ChooseOption widget = null;
            if (HighLogic.LoadedSceneIsEditor)
            {
                widget = (UI_ChooseOption)module.Fields[fieldName].uiControlEditor;
            }
            else if (HighLogic.LoadedSceneIsFlight)
            {
                widget = (UI_ChooseOption)module.Fields[fieldName].uiControlFlight;
            }
            else { return; }
            if (widget == null) { return; }
            widget.display = display;
            widget.options = options;
            if (forceUpdate && widget.partActionItem != null)
            {
                UIPartActionChooseOption control = (UIPartActionChooseOption)widget.partActionItem;
                var t = widget.onFieldChanged;
                widget.onFieldChanged = null;
                int index = Array.IndexOf(options, forceVal);
                control.slider.minValue = 0;
                control.slider.maxValue = options.Length - 1;
                control.slider.value = index;
                control.OnValueChanged(0);
                widget.onFieldChanged = t;
            }
        }

        public static void updateUIScaleEditControl(this PartModule module, string fieldName, float[] intervals, float[] increments, bool forceUpdate, float forceValue = 0)
        {
            UI_ScaleEdit widget = null;
            if (HighLogic.LoadedSceneIsEditor)
            {
                widget = (UI_ScaleEdit)module.Fields[fieldName].uiControlEditor;
            }
            else if (HighLogic.LoadedSceneIsFlight)
            {
                widget = (UI_ScaleEdit)module.Fields[fieldName].uiControlFlight;
            }
            else
            {
                return;
            }
            if (widget == null)
            {
                return;
            }
            widget.intervals = intervals;
            widget.incrementSlide = increments;
            if (forceUpdate && widget.partActionItem != null)
            {
                UIPartActionScaleEdit ctr = (UIPartActionScaleEdit)widget.partActionItem;
                var t = widget.onFieldChanged;
                widget.onFieldChanged = null;
                ctr.inc.onToggle.RemoveAllListeners();
                ctr.dec.onToggle.RemoveAllListeners();
                ctr.slider.onValueChanged.RemoveAllListeners();
                ctr.Setup(ctr.Window, module.part, module, HighLogic.LoadedSceneIsEditor ? UI_Scene.Editor : UI_Scene.Flight, widget, module.Fields[fieldName]);
                widget.onFieldChanged = t;
            }
        }

        public static void updateUIScaleEditControl(this PartModule module, string fieldName, float min, float max, float increment, bool flight, bool editor, bool forceUpdate, float forceValue = 0)
        {
            BaseField field = module.Fields[fieldName];
            if (increment <= 0)//div/0 error
            {
                field.guiActive = false;
                field.guiActiveEditor = false;
                return;
            }
            float seg = (max - min) / increment;
            int numOfIntervals = (int)Math.Round(seg) + 1;
            float sliderInterval = increment * 0.05f;
            float[] intervals = new float[numOfIntervals];
            float[] increments = new float[numOfIntervals];
            UI_Scene scene = HighLogic.LoadedSceneIsFlight ? UI_Scene.Flight : UI_Scene.Editor;
            if (numOfIntervals <= 1)//not enough data...
            {
                field.guiActive = false;
                field.guiActiveEditor = false;
                MonoBehaviour.print("ERROR: Not enough data to create intervals: " + min + " : " + max + " :: " + increment);
            }
            else
            {
                field.guiActive = flight;
                field.guiActiveEditor = editor;
                intervals = new float[numOfIntervals];
                increments = new float[numOfIntervals];
                for (int i = 0; i < numOfIntervals; i++)
                {
                    intervals[i] = min + (increment * (float)i);
                    increments[i] = sliderInterval;
                }
                module.updateUIScaleEditControl(fieldName, intervals, increments, forceUpdate, forceValue);
            }
        }

        public static void updateUIScaleEditControl(this PartModule module, string fieldName, float value)
        {
            UI_ScaleEdit widget = null;
            if (HighLogic.LoadedSceneIsEditor)
            {
                widget = (UI_ScaleEdit)module.Fields[fieldName].uiControlEditor;
            }
            else if (HighLogic.LoadedSceneIsFlight)
            {
                widget = (UI_ScaleEdit)module.Fields[fieldName].uiControlFlight;
            }
            else
            {
                return;
            }
            if (widget == null || widget.partActionItem == null)
            {
                return;
            }
            UIPartActionScaleEdit ctr = (UIPartActionScaleEdit)widget.partActionItem;
            var t = widget.onFieldChanged;
            widget.onFieldChanged = null;
            ctr.inc.onToggle.RemoveAllListeners();
            ctr.dec.onToggle.RemoveAllListeners();
            ctr.slider.onValueChanged.RemoveAllListeners();
            ctr.Setup(ctr.Window, module.part, module, HighLogic.LoadedSceneIsEditor ? UI_Scene.Editor : UI_Scene.Flight, widget, module.Fields[fieldName]);
            widget.onFieldChanged = t;
        }

        /// <summary>
        /// Performs the input delegate onto the input part module and any modules found in symmetry counerparts.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="module"></param>
        /// <param name="action"></param>
        public static void actionWithSymmetry<T>(this T module, Action<T> action) where T : PartModule
        {
            action(module);
            forEachSymmetryCounterpart(module, action);
        }

        /// <summary>
        /// Performs the input delegate onto any modules found in symmetry counerparts. (does not effect this.module)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="module"></param>
        /// <param name="action"></param>
        public static void forEachSymmetryCounterpart<T>(this T module, Action<T> action) where T : PartModule
        {
            int index = module.part.Modules.IndexOf(module);
            int len = module.part.symmetryCounterparts.Count;
            for (int i = 0; i < len; i++)
            {
                action((T)module.part.symmetryCounterparts[i].Modules[index]);
            }
        }

        #endregion
    }
}
