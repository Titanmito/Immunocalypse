using UnityEngine;

public class Lymphocyte : MonoBehaviour {
	// This component marks the lymphocyte entity and keeps its type (1 or 2) so we can find the allies we want.
	// There are two types of Lymphocytes:
	// type 1 -> creates anticorps that attacks virus
	// type 2 -> creates anticorps attacks bacterie
	public int type;
}