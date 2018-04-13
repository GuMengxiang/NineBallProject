using UnityEngine;
using System.Collections;
using System.Runtime.Serialization;
using System;

public class PurchaseInfo : ISerializable,IEquatable<PurchaseInfo> {
    #region Fields
    private string 	_orderId;
    private string 	_productId;
	private string 	_purchaseState;
	private string 	_purchasePayload;
	private long 	_purchaseTime;
    #endregion
	
    #region Properties
    public string orderId
    {
        get { return _orderId; }
        set { _orderId = value; }
    }
    public string productId
    {
        get { return _productId; }
        set { _productId = value; }
    }
    public string purchaseState
    {
        get { return _purchaseState; }
        set { _purchaseState = value; }
    }
    public string purchasePayload
    {
        get { return _purchasePayload; }
        set { _purchasePayload = value; }
    }
    public long purchaseTime
    {
        get { return _purchaseTime; }
        set { _purchaseTime = value; }
    }

	#endregion
    #region Functions
	public PurchaseInfo()
	{
		_orderId = "";
	}

    public void GetObjectData(SerializationInfo info, StreamingContext ctxt)
    {
    }
	
	public bool Equals(PurchaseInfo other)//Only interested in purchases in this demo
	{
		if(this.orderId == other.orderId)
			return true;
		else
			return false;
	}
	#endregion
}
