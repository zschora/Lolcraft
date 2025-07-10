using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

public class StateMachine
{
    private IState currentState;
    private IState prevState;
    private Dictionary<Type, IState> states = new();
    
    // Добавление состояния в машину состояний
    public void AddState(IState state)
    {
        Type stateType = state.GetType();
        states.TryAdd(stateType, state);
    }
    
    // Смена состояния
    public void ChangeState<T>() where T : IState
    {
        System.Type stateType = typeof(T);
        
        if (!states.ContainsKey(stateType))
        {
            Debug.LogError($"Состояние {stateType} не найдено в StateMachine!");
            return;
        }
        
        // Выходим из текущего состояния
        if (currentState != null)
        {
            currentState.Exit();
        }

        // Меняем состояние
        currentState = states[stateType];
        
        // Входим в новое состояние
        currentState.Enter();
        
        Debug.Log($"Состояние изменено на: {stateType.Name}");
    }

    // Вернуться в предыдущее состояние
    public void RevertState()
    {
        if (prevState != null)
        {
            currentState.Exit();
            currentState = prevState;
            currentState.Enter();

            Debug.Log($"Состояние возвращенно");
        }
    }
    
    // Обновление текущего состояния
    public void Update()
    {
        if (currentState != null)
        {
            currentState.Execute();
        }
    }
    
    // Получить текущее состояние
    public IState GetCurrentState()
    {
        return currentState;
    }
    
    // Проверка текущего состояния
    public bool IsInState<T>() where T : IState
    {
        return currentState != null && currentState.GetType() == typeof(T);
    }
}