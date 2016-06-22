using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public abstract class User
{
    protected List<string> licences = new List<string>();

    protected User()
    {
    }

    public virtual void ClearInventory()
    {
        this.Gold = 0;
        this.Chests = 0;
        this.ClearLicences();
    }

    public virtual void ClearLicences()
    {
        this.Licences = new List<string>();
    }

    public virtual void Reset()
    {
        this.DisplayName = string.Empty;
        this.Id = string.Empty;
        this.ClearInventory();
    }

    public virtual int Chests { get; set; }

    public virtual bool DailyQuestAvailable { get; set; }

    public virtual string DailyQuestCurrent { get; set; }

    public virtual long DailyQuestTimeSet { get; set; }

    public virtual double DailyQuestTimeTillReset { get; set; }

    public virtual string DisplayName { get; set; }

    public virtual int Gold { get; set; }

    public virtual long GoldResetTimeSet { get; set; }

    public virtual bool GoldSubAvailable { get; set; }

    public virtual int GoldSubDaysRemaining { get; set; }

    public virtual double GoldSubTimeTillReset { get; set; }

    public virtual string Id { get; set; }

    public virtual List<string> Licences
    {
        get
        {
            if (this.licences == null)
            {
                this.licences = new List<string>();
            }
            return this.licences;
        }
        set
        {
            this.licences = value;
        }
    }

    public virtual string PromotionalQuestCurrent { get; set; }
}

