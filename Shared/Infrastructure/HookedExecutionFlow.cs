using System;

namespace JsonBridgeEF.Shared.Dag.ExecutionFlow
{
    /// <summary>
    /// Costruttore fluente per orchestrare un flusso di esecuzione con hook opzionali prima e dopo ciascuna fase.
    /// </summary>
    /// <typeparam name="T">Tipo dell'argomento passato a ogni fase.</typeparam>
    internal static class HookedExecutionFlow<T>
    {
        public static IHookedExecutionBuilderWithValidation<T> WithCore(Action<T> coreAction)
        {
            return new Builder(coreAction);
        }

        private sealed class Builder :
            IHookedExecutionBuilderWithValidation<T>,
            IHookedExecutionBuilderAfterValidation<T>,
            IHookedExecutionBuilderAfterPostValidation<T>
        {
            private readonly Action<T> _coreAction;

            private Action<T>? _preValidation;
            private Action<T>? _postValidation;

            private Action<T>? _onBeforePreValidation;
            private Action<T>? _onAfterPreValidation;
            private Action<T>? _onBeforeCoreAction;
            private Action<T>? _onAfterCoreAction;
            private Action<T>? _onBeforePostValidation;
            private Action<T>? _onAfterPostValidation;

            public Builder(Action<T> coreAction)
            {
                _coreAction = coreAction ?? throw new ArgumentNullException(nameof(coreAction));
            }

            public IHookedExecutionBuilderAfterValidation<T> WithValidation(Action<T> preValidation)
            {
                _preValidation = preValidation;
                return this;
            }

            public IHookedExecutionBuilderAfterPostValidation<T> WithPostValidation(Action<T> validate)
            {
                _postValidation = validate;
                return this;
            }

            public IHookedExecutionBuilderAfterPreValidation<T> OnBeforePreValidation(Action<T> hook)
            {
                _onBeforePreValidation = hook;
                return this;
            }

            public IHookedExecutionBuilderAfterPreValidation<T> OnAfterPreValidation(Action<T> hook)
            {
                _onAfterPreValidation = hook;
                return this;
            }

            public IHookedExecutionBuilderAfterCore<T> OnBeforeCoreAction(Action<T> hook)
            {
                _onBeforeCoreAction = hook;
                return this;
            }

            public IHookedExecutionBuilderAfterCore<T> OnAfterCoreAction(Action<T> hook)
            {
                _onAfterCoreAction = hook;
                return this;
            }

            public IHookedExecutionBuilderAfterPostValidation<T> OnBeforePostValidation(Action<T> hook)
            {
                _onBeforePostValidation = hook;
                return this;
            }

            public IHookedExecutionBuilderAfterPostValidation<T> OnAfterPostValidation(Action<T> hook)
            {
                _onAfterPostValidation = hook;
                return this;
            }

            public void Execute(T context)
            {
                _onBeforePreValidation?.Invoke(context);
                _preValidation?.Invoke(context);
                _onAfterPreValidation?.Invoke(context);

                _onBeforeCoreAction?.Invoke(context);
                _coreAction(context);
                _onAfterCoreAction?.Invoke(context);

                if (_postValidation is not null)
                {
                    _onBeforePostValidation?.Invoke(context);
                    _postValidation(context);
                    _onAfterPostValidation?.Invoke(context);
                }
            }
        }
    }

    internal interface IHookedExecutionBuilderWithValidation<T>
    {
        IHookedExecutionBuilderAfterValidation<T> WithValidation(Action<T> preValidation);
    }

    internal interface IHookedExecutionBuilderAfterValidation<T> : IHookedExecutionBuilderAfterPreValidation<T>
    {
        IHookedExecutionBuilderAfterPostValidation<T> WithPostValidation(Action<T> validate);
    }

    internal interface IHookedExecutionBuilderAfterPreValidation<T> : IHookedExecutionBuilderAfterCore<T>
    {
        IHookedExecutionBuilderAfterPreValidation<T> OnBeforePreValidation(Action<T> hook);
        IHookedExecutionBuilderAfterPreValidation<T> OnAfterPreValidation(Action<T> hook);
    }

    internal interface IHookedExecutionBuilderAfterCore<T>
    {
        IHookedExecutionBuilderAfterCore<T> OnBeforeCoreAction(Action<T> hook);
        IHookedExecutionBuilderAfterCore<T> OnAfterCoreAction(Action<T> hook);
        void Execute(T context);
    }

    internal interface IHookedExecutionBuilderAfterPostValidation<T>
    {
        IHookedExecutionBuilderAfterPostValidation<T> OnBeforePostValidation(Action<T> hook);
        IHookedExecutionBuilderAfterPostValidation<T> OnAfterPostValidation(Action<T> hook);
        void Execute(T context);
    }
}