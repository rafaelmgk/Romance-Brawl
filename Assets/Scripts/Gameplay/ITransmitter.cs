using System;

public interface ITransmitter {
	void Notify(Enum notificationType, object actionParams = null);
}
