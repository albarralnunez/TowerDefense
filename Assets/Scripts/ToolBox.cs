using System.Collections;
using System.Collections.Generic;

public class Toolbox : Singleton<Toolbox> {
	protected Toolbox () {} // guarantee this will be always a singleton only - can't use the constructor!

	public List<int> EnemyBusy = new List<int>();
	
	void Awake () {
		// Your initialization code here

	}
	
	// (optional) allow runtime registration of global objects
	/*static public T RegisterComponent<T> () {
		return Instance.GetOrAddComponent<T>();
	}*/
}
