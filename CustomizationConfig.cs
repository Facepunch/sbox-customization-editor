using System.Collections.Generic;

namespace Facepunch.CustomizationTool;

public class CustomizationConfig
{

	public int CategoryIdAccumulator { get; set; }
	public int PartIdAccumulator { get; set; }
	public List<CustomizationCategory> Categories { get; set; } = new();
	public List<CustomizationPart> Parts { get; set; } = new();

}

public class CustomizationCategory
{

	[ObjectForm.ReadOnly]
	public int Id { get; set; }
	public string DisplayName { get; set; }
	[ObjectForm.ImagePicker]
	public string IconPath { get; set; }
	[ObjectForm.PartDropdown]
	public int DefaultPartId { get; set; }

}

public class CustomizationPart
{

	[ObjectForm.ReadOnly]
	public int Id { get; set; }
	[ObjectForm.CategoryDropdown]
	public int CategoryId { get; set; }
	public string DisplayName { get; set; }
	[ObjectForm.ImagePicker]
	public string IconPath { get; set; }
	[ObjectForm.AssetPicker]
	public string AssetPath { get; set; }

}
