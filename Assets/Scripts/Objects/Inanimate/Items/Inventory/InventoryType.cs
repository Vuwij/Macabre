/// <summary>
/// The Inventory types, currently the ones that are only avalbale use overriding to define the inventory sizes
/// </summary>
namespace Objects.Items.Inventory
{
    public class Object : Inventory {

	}

	public class ObjectASmall : Object {
		new public int classALimit = 2;
		new public int classBLimit = 0;

	}

	public class ObjectAMedium : Object {
		new public int classALimit = 4;
		new public int classBLimit = 0;

	}

	public class ObjectALarge : Object {
		new public int classALimit = 6;
		new public int classBLimit = 0;

	}

	public class ObjectB : Object {
		new public int classALimit = 0;
		new public int classBLimit = 1;

	}

	public class Character : Inventory {
		new public int classALimit = 6;
		new public int classBLimit = 2;

	}

	public class Merchant : Character {
		new public int classALimit = 20;
		new public int classBLimit = 2;

	}

	public class Player : Character {
		new public int classALimit = 6;
		new public int classBLimit = 2;

	}
}
