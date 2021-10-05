using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class CustomVariableStorage : Yarn.Unity.VariableStorageBehaviour
{
    [SerializeField] private InMemoryVariableStorage variableStorage;

    // Store a value into a variable
    public override void SetValue(string variableName, Yarn.Value value)
    {
        // 'variableName' is the name of the variable that 'value' 
        // should be stored in.
        foreach (var item in variableStorage.defaultVariables)
        {
            if ("$" + item.name == variableName)
            {
                item.value = value.AsString;
            }
        }
    }

    // Return a value, given a variable name
    public override Yarn.Value GetValue(string variableName)
    {
        foreach (var item in variableStorage.defaultVariables)
        {
            if ("$" + item.name == variableName)
            {
                return new Yarn.Value(item.value);
            }
        }
        return new Yarn.Value();
    }

    // Return to the original state
    public override void ResetToDefaults()
    {
        return;
    }
}
