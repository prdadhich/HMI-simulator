using UnityEngine;
using System.Collections;
using UnityStandardAssets.Vehicles.Car;

public class DynamicCarController : MonoBehaviour {

	internal enum timeInterval{
		Fixed,
		Rand
	}
	
	internal enum TreatmentPeriod{
		Fixed,
		Rand
	}


	[SerializeField] private timeInterval m_timeInterval = timeInterval.Fixed;
	[SerializeField] private TreatmentPeriod m_TreatmentPeriod = TreatmentPeriod.Fixed;
	public bool changeSpeed = false;
	public float DurationInterval = 5.0f;
	public float TreatmentDuration = 5.0f;
	public float speed = 60.0f;
	public bool speedchange;


	private float temp;
	private Rigidbody rb;
	private CarController m_Car; // the car controller we want to use

	private void Awake () {
		
		rb = GetComponent<Rigidbody> ();
		m_Car = GetComponent<CarController>();

	}
	// Use this for initialization
	void Start () {

	//	StartCoroutine (InvokeChangeSpeed());
        temp = m_Car.MaxSpeed;


	}


	//public IEnumerator InvokeChangeSpeed(){
	//	while (true) {

 //           // increase the speed after duration interval elapsed
 //           /*
 //            * 1. Initial change speed = false for constant speed will remain constant
 //            * 2. intial change speed = true for varying speed  will change to constant
 //            */
	//		if (changeSpeed) {
	//			yield return new WaitForSeconds (DurationInterval);
	//			speedchange = true;
	//			temp = m_Car.MaxSpeed;
	//			m_Car.MaxSpeed = speed;

	//		}

 //           // decrease the speed after treatment duration finishes
	//		yield return new WaitForSeconds (TreatmentDuration);
	//		m_Car.MaxSpeed = temp;
	//		speedchange = false;
			
			
	//	}
		
	//}
}
