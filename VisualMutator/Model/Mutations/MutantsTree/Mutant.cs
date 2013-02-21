namespace VisualMutator.Model.Mutations.Structure
{
    using System;
    using System.Collections.Generic;
    using CommonUtilityInfrastructure;
    using Microsoft.Cci;
    using VisualMutator.Extensibility;
    using VisualMutator.Model.Tests;

    public class Mutant : MutationNode
    {
        private readonly int _id;

        private readonly MutationTarget _mutationTarget;
        private readonly List<MutationTarget> _commonTargets;

        public List<MutationTarget> CommonTargets
        {
            get { return _commonTargets; }
        }

        public List<IModule> MutatedModules
        {
            get;
            set;
        }

        public MutationTarget MutationTarget
        {
            get
            {
                return _mutationTarget;
            }
        }
        public ExecutedOperator ExecutedOperator
        {
            get
            {
                return Parent.CastTo<ExecutedOperator>();
            }
        }
        public Mutant(int id, ExecutedOperator parent, MutationTarget mutationTarget, List<MutationTarget> commonTargets)
            : base( "Mutant", false)
        {
            _id = id;
            _mutationTarget = mutationTarget;
            _commonTargets = commonTargets;

            _mutantTestSession  = new MutantTestSession();

            Parent = parent;
        }

        public int Id
        {
            get
            {
                return _id;
            }
        }
        protected override void SetState(MutantResultState value, bool updateChildren, bool updateParent)
        {
            string stateText =
                Switch.Into<string>().From(value)
                .Case(MutantResultState.Untested, "Untested")
                .Case(MutantResultState.Tested, "Executing tests...")
                .Case(MutantResultState.Killed, () =>
                {
                    return Switch.Into<string>().From(KilledSubstate)
                        .Case(MutantKilledSubstate.Normal, ()=>"Killed by {0} tests".Formatted(NumberOfFailedTests))
                        .Case(MutantKilledSubstate.Inconclusive, ()=>"Killed by {0} tests".Formatted(NumberOfFailedTests))
                        .Case(MutantKilledSubstate.Cancelled, ()=>"Cancelled")
                        .GetResult();
                })
                .Case(MutantResultState.Live, "Live")
                .Case(MutantResultState.Error, () => MutantTestSession.ErrorDescription)
                .GetResult();

            DisplayedText = "#{0} {1}".Formatted(Id, stateText);
            base.SetState(value, updateChildren, updateParent);
        }

        private int _numberOfFailedTests;

        public int NumberOfFailedTests
        {
            get
            {
                return _numberOfFailedTests;
            }
            set
            {
                SetAndRise(ref _numberOfFailedTests, value, () => NumberOfFailedTests);
            }
        }

        private string _displayedText;

        public string DisplayedText  
        {
            get
            {
                return _displayedText;
            }
            set
            {
                SetAndRise(ref _displayedText, value, () => DisplayedText);
            }
        }

        private MutantTestSession _mutantTestSession;

        public MutantTestSession MutantTestSession
        {
            get
            {
                return _mutantTestSession;
            }

        }

        public MutantKilledSubstate KilledSubstate { get; set; }

    }

    public enum MutantKilledSubstate
    {
        Normal,
        Inconclusive,
        Cancelled,
    }
}