using UnityEngine;
using UnityEditor;

/// <summary>
/// 2013-07-08
/// ULTIMATE RAGDOLL GENERATOR V4.2
/// © THE ARC GAMES STUDIO 2013
/// 
/// Utility class used primarily to maintain compatibility with Unity4 during model import.
/// This is required in particular for dismemberment purposes
/// </summary>
class MeshPostprocessor : AssetPostprocessor {

	void OnPreprocessModel () {
		(assetImporter as ModelImporter).optimizeMesh = false;
	}

}
