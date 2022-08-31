using NHibernate;
using NHibernate.Mapping.ByCode;
using NHibernate.Mapping.ByCode.Conformist;
using PycApi.Model;

namespace PycApi.Mapping
{
    //Veri tabanı ve class içindeki tablonun birbirleri ile eşlenmesi
    public class VehicleMap : ClassMapping<Vehicle>
    {
        public VehicleMap()
        {
            Id(x => x.Id, x =>
            {
                x.Type(NHibernateUtil.Int64);
                x.Column("id");
                x.Generator(Generators.Increment);
            });  

            Property(b => b.VehicleName, x =>
            {
                x.Length(50);
                x.Type(NHibernateUtil.String);
                x.NotNullable(true);
            });
            Property(b => b.VehiclePlate, x =>
            {
                x.Length(14);
                x.Type(NHibernateUtil.String);
                x.NotNullable(true);
            });
           
            Table("Vehicle");
        }
    }
}