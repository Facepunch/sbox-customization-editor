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

	[CustomizationObjectForm.ReadOnly]
	public int Id { get; set; }
	public string DisplayName { get; set; }
	[CustomizationObjectForm.FilePicker]
	public string IconPath { get; set; }
	[CustomizationObjectForm.PartDropdown]
	public int DefaultPartId { get; set; }

}

public class CustomizationPart
{

	[CustomizationObjectForm.ReadOnly]
	public int Id { get; set; }
	[CustomizationObjectForm.CategoryDropdown]
	public int CategoryId { get; set; }
	public string DisplayName { get; set; }
	[CustomizationObjectForm.FilePicker]
	public string IconPath { get; set; }
	[CustomizationObjectForm.AssetPicker]
	public string AssetPath { get; set; }

}
