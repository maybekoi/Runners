using UnityEngine;

public class ykTextureSheetSharedMaterialAnimation : ykTextureSheetAnimation
{
	protected override Material GetMaterial()
	{
		return base.GetComponent<Renderer>().sharedMaterial;
	}

	protected override bool IsValidChange()
	{
		return false;
	}
}
