﻿namespace VisualMutator.Extensibility
{
    #region

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using log4net;
    using Microsoft.Cci;
    using UsefulTools.ExtensionMethods;

    #endregion

    public class VisualCodeVisitorBack : VisualCodeVisitor
    {
        private ILog _log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private readonly ICollection<MutationTarget> _mutationTargets;//TODO: remove, its ambigius with base class field of the same name
        private readonly List<MutationTarget> _sharedTargets;
        //List of objects found in this model tree corresponding to mutation targets
        private readonly List<Tuple<object, MutationTarget>> _targetAstObjects;
        private readonly List<object> _sharedAstObjects;

        public List<object> SharedAstObjects
        {
            get { return _sharedAstObjects; }
        }

        public List<Tuple<object, MutationTarget>> TargetAstObjects
        {
            get { return _targetAstObjects; }
        }

        public VisualCodeVisitorBack(ICollection<MutationTarget> mutationTargets, 
            List<MutationTarget> sharedTargets, IModule module)
            : base(new OperatorCodeVisitor(), module)
        {
            _mutationTargets = mutationTargets;
            _sharedTargets = sharedTargets;
            _targetAstObjects = new List<Tuple<object, MutationTarget>>();
            _sharedAstObjects = new List<object>();
        }


        protected override bool Process(object obj)
        {
            base.Process(obj);
         //   _log.Debug("Process back: " + TreeObjectsCounter + " " + Formatter.Format(obj) + " : " + obj.GetHashCode());
            //Are we processing an object corresponding to any mutation target?
            var target = _mutationTargets.SingleOrDefault(t => t.CounterValue == TreeObjectsCounter);
            
        //    _log.Debug("Creating pair: " + TreeObjectsCounter + " " + Formatter.Format(obj) + " <===> " + target);
            if (target != null)
            {
                _targetAstObjects.Add(Tuple.Create(obj, target));
            }
            
            if (_sharedTargets.Any(t => t.CounterValue == TreeObjectsCounter))
            {
                _sharedAstObjects.Add(obj);
            }
            return false;
        }


        public override void PostProcess()
        {
            foreach (var mutationTarget in _mutationTargets)
            {
                if (mutationTarget.ModuleName == _traversedModule.Name.Value)
                {
                    //TODO: do better. now they can be null for changeless mutant
                    if (mutationTarget.VariantObjectsIndices != null && AllAstObjects != null)
                    {
                        mutationTarget.Variant.AstObjects = mutationTarget.VariantObjectsIndices
                        .MapValues((key, val) => AllAstObjects[val]);
                        if (!AllAstObjects.ContainsKey(mutationTarget.MethodIndex))
                        {
                            Debugger.Break();
                        }
                        mutationTarget.MethodMutated = (IMethodDefinition)AllAstObjects[mutationTarget.MethodIndex];
                    }
                }
            }
        }
    }
}