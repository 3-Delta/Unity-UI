using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FSM
{
	public enum EState
	{
		Open, 
		Close,
	}

	public class StateOpen : State
	{
        public StateOpen() : base("StateOpen") { }

        public override void Enter(IState prev)
        {
			Debug.LogError("Enter :" + "StateOpen");
        }
        public override void Exit(IState next)
        {
			Debug.LogError("Exit :" + "StateOpen");
        }
        public override void Update(float deltaTime)
        {
			Debug.LogError("Update :" + "StateOpen");
        }
	}
	
	public class StateClose : State
	{
        public StateClose() : base("StateClose") { }

        public override void Enter(IState prev)
        {
			Debug.LogError("Enter :" + "StateClose");
        }
        public override void Exit(IState next)
        {
			Debug.LogError("Exit :" + "StateClose");
        }
        public override void Update(float deltaTime)
        {
			Debug.LogError("Update :" + "StateClose");
        }
	}
	
	public class Open2Close : Transition
	{
		public Open2Close(string name, IState from = null, IState to = null) : base(name, from, to){}
		public override bool ShouleEnd() { return BS_StateTest.can != true; }
        public override bool ShouldBegin()
        {
            return BS_StateTest.can != true;
        }
	}
	public class Close2Open : Transition
	{
		public Close2Open(string name, IState from = null, IState to = null) : base(name, from, to){}
		public override bool ShouleEnd() { return BS_StateTest.can == true; }
        public override bool ShouldBegin()
        {
            return BS_StateTest.can == true;
        }
	}
	
	/*
	public class BS_StateTestEvent : MonoBehaviour 
	{
		public BS_StateMachine fsm { get; private set; } = null;
		
		private void Awake()
		{
			StateOpen stOpen = new StateOpen();
			StateClose stClose = new StateClose();
			
			Open2Close o2c = new Open2Close("Open2Close", stOpen, stClose);
			Close2Open c2o = new Close2Open("Close2Open", stClose, stOpen);
			
			stOpen.AddTransition(o2c);
			stClose.AddTransition(c2o);
			
			fsm = new BS_StateMachine(stClose);
			fsm.Add(stOpen);
			fsm.Add(stClose);
		}
		
		private void Start()
		{
			
		}
		
		private void Update()
		{
			fsm?.Update(Time.deltaTime);
		}
	}
	*/

    public class BS_StateTest : MonoBehaviour
    {
	    public StateMachine fsm { get; protected set; } = null;
	    public static bool can = false;
	
	    private void Awake()
	    {
		    StateOpen stOpen = new StateOpen();
		    StateClose stClose = new StateClose();
		
		    Open2Close o2c = new Open2Close("Open2Close", stOpen, stClose);
		    Close2Open c2o = new Close2Open("Close2Open", stClose, stOpen);
		
		    stOpen.AddTransition(o2c);
		    stClose.AddTransition(c2o);
		
		    fsm = new StateMachine("FSM", stClose);
		    fsm.Add("StateOpen", stOpen);
		    fsm.Add("StateClose", stClose);

            fsm.Enter(null);
	    }
	
	    public void Update()
	    {
		    fsm?.Update(Time.deltaTime);
		
		    if(Input.GetKeyDown(KeyCode.A))
		    {
                Invoke("AAA", 10f);
		    }
            else if(Input.GetKeyDown(KeyCode.B))
		    {
                Invoke("BBB", 7f);
            }
	    }

        private void AAA()
        {
            can = false;
        }
        private void BBB()
        {
            can = true;
        }
    }
}

