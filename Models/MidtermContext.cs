using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace LeSheTuanGo.Models
{
    public partial class MidtermContext : DbContext
    {
        public MidtermContext()
        {
        }

        public MidtermContext(DbContextOptions<MidtermContext> options)
            : base(options)
        {
        }

        public virtual DbSet<CategoryRef> CategoryRefs { get; set; }
        public virtual DbSet<ChatMessageRecord> ChatMessageRecords { get; set; }
        public virtual DbSet<CityRef> CityRefs { get; set; }
        public virtual DbSet<DistrictRef> DistrictRefs { get; set; }
        public virtual DbSet<GarbageServiceOffer> GarbageServiceOffers { get; set; }
        public virtual DbSet<GarbageServiceUseRecord> GarbageServiceUseRecords { get; set; }
        public virtual DbSet<GarbageSpotAlert> GarbageSpotAlerts { get; set; }
        public virtual DbSet<GarbageTruckSpot> GarbageTruckSpots { get; set; }
        public virtual DbSet<Member> Members { get; set; }
        public virtual DbSet<Notification> Notifications { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderBuyRecord> OrderBuyRecords { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<RangeRef> RangeRefs { get; set; }
        public virtual DbSet<Route> Routes { get; set; }
        public virtual DbSet<ServiceTypeRef> ServiceTypeRefs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Data Source=.;Initial Catalog=Midterm;Integrated Security=True");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "Chinese_Taiwan_Stroke_CI_AS");

            modelBuilder.Entity<CategoryRef>(entity =>
            {
                entity.HasKey(e => e.CategoryId);

                entity.Property(e => e.CategoryId)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("CategoryID");

                entity.Property(e => e.CategoryName)
                    .IsRequired()
                    .HasMaxLength(5);
            });

            modelBuilder.Entity<ChatMessageRecord>(entity =>
            {
                entity.HasKey(e => e.MessageId)
                    .HasName("PK_ChatMessages");

                entity.Property(e => e.MessageId).HasColumnName("MessageID");

                entity.Property(e => e.GroupId).HasColumnName("GroupID");

                entity.Property(e => e.Message).IsRequired();

                entity.Property(e => e.SentMemberId).HasColumnName("SentMemberID");

                entity.Property(e => e.SentTime).HasColumnType("datetime");

                entity.HasOne(d => d.SentMember)
                    .WithMany(p => p.ChatMessageRecords)
                    .HasForeignKey(d => d.SentMemberId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ChatMessageRecords_Members");
            });

            modelBuilder.Entity<CityRef>(entity =>
            {
                entity.HasKey(e => e.CityId)
                    .HasName("PK_Citys");

                entity.Property(e => e.CityId)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("CityID");

                entity.Property(e => e.CityName)
                    .IsRequired()
                    .HasMaxLength(3);
            });

            modelBuilder.Entity<DistrictRef>(entity =>
            {
                entity.HasKey(e => e.DistrictId)
                    .HasName("PK_Districts");

                entity.Property(e => e.DistrictId).HasColumnName("DistrictID");

                entity.Property(e => e.CityId).HasColumnName("CityID");

                entity.Property(e => e.DistrictName)
                    .IsRequired()
                    .HasMaxLength(4);

                entity.HasOne(d => d.City)
                    .WithMany(p => p.DistrictRefs)
                    .HasForeignKey(d => d.CityId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_DistrictRefs_CityRefs");
            });

            modelBuilder.Entity<GarbageServiceOffer>(entity =>
            {
                entity.HasKey(e => e.GarbageServiceId)
                    .HasName("PK_GarbageEmptyServices");

                entity.Property(e => e.GarbageServiceId).HasColumnName("GarbageServiceID");

                entity.Property(e => e.Address)
                    .IsRequired()
                    .HasMaxLength(30);

                entity.Property(e => e.DistrictId).HasColumnName("DistrictID");

                entity.Property(e => e.EndTime).HasColumnType("datetime");

                entity.Property(e => e.GoRangeId).HasColumnName("GoRangeID");

                entity.Property(e => e.HostMemberId).HasColumnName("HostMemberID");

                entity.Property(e => e.IsActive).HasColumnName("isActive");

                entity.Property(e => e.L120available).HasColumnName("L120Available");

                entity.Property(e => e.L120maxCount).HasColumnName("L120MaxCount");

                entity.Property(e => e.L14available).HasColumnName("L14Available");

                entity.Property(e => e.L14maxCount).HasColumnName("L14MaxCount");

                entity.Property(e => e.L25available).HasColumnName("L25Available");

                entity.Property(e => e.L25maxCount).HasColumnName("L25MaxCount");

                entity.Property(e => e.L33available).HasColumnName("L33Available");

                entity.Property(e => e.L33maxCount).HasColumnName("L33MaxCount");

                entity.Property(e => e.L3available).HasColumnName("L3Available");

                entity.Property(e => e.L3maxCount).HasColumnName("L3MaxCount");

                entity.Property(e => e.L5available).HasColumnName("L5Available");

                entity.Property(e => e.L5maxCount).HasColumnName("L5MaxCount");

                entity.Property(e => e.L75available).HasColumnName("L75Available");

                entity.Property(e => e.L75maxCount).HasColumnName("L75MaxCount");

                entity.Property(e => e.Latitude).HasColumnType("numeric(9, 7)");

                entity.Property(e => e.Longitude).HasColumnType("numeric(10, 7)");

                entity.Property(e => e.ServiceTypeId).HasColumnName("ServiceTypeID");

                entity.Property(e => e.StartTime).HasColumnType("datetime");

                entity.HasOne(d => d.District)
                    .WithMany(p => p.GarbageServiceOffers)
                    .HasForeignKey(d => d.DistrictId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_GarbageServiceOffers_DistrictRefs");

                entity.HasOne(d => d.GoRange)
                    .WithMany(p => p.GarbageServiceOffers)
                    .HasForeignKey(d => d.GoRangeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_GarbageServiceOffers_RangeRefs");

                entity.HasOne(d => d.HostMember)
                    .WithMany(p => p.GarbageServiceOffers)
                    .HasForeignKey(d => d.HostMemberId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_GarbageServiceOffers_Members");

                entity.HasOne(d => d.ServiceType)
                    .WithMany(p => p.GarbageServiceOffers)
                    .HasForeignKey(d => d.ServiceTypeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_GarbageServiceOffers_ServiceTypeRefs");
            });

            modelBuilder.Entity<GarbageServiceUseRecord>(entity =>
            {
                entity.HasKey(e => e.ServiceUseRecordId)
                    .HasName("PK_ServiceSubscribes");

                entity.Property(e => e.ServiceUseRecordId).HasColumnName("ServiceUseRecordID");

                entity.Property(e => e.ComeAddress)
                    .IsRequired()
                    .HasMaxLength(30);

                entity.Property(e => e.ComeDistrictId).HasColumnName("ComeDistrictID");

                entity.Property(e => e.GarbageServiceOfferId).HasColumnName("GarbageServiceOfferID");

                entity.Property(e => e.L120count).HasColumnName("L120Count");

                entity.Property(e => e.L14count).HasColumnName("L14Count");

                entity.Property(e => e.L25count).HasColumnName("L25Count");

                entity.Property(e => e.L33count).HasColumnName("L33Count");

                entity.Property(e => e.L3count).HasColumnName("L3Count");

                entity.Property(e => e.L5count).HasColumnName("L5Count");

                entity.Property(e => e.L75count).HasColumnName("L75Count");

                entity.Property(e => e.MemberId).HasColumnName("MemberID");

                entity.HasOne(d => d.ComeDistrict)
                    .WithMany(p => p.GarbageServiceUseRecords)
                    .HasForeignKey(d => d.ComeDistrictId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_GarbageServiceUseRecords_DistrictRefs");

                entity.HasOne(d => d.GarbageServiceOffer)
                    .WithMany(p => p.GarbageServiceUseRecords)
                    .HasForeignKey(d => d.GarbageServiceOfferId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_GarbageEmptySubscribes_GarbageEmptyServices");

                entity.HasOne(d => d.Member)
                    .WithMany(p => p.GarbageServiceUseRecords)
                    .HasForeignKey(d => d.MemberId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_GarbageEmptySubscribes_Members");
            });

            modelBuilder.Entity<GarbageSpotAlert>(entity =>
            {
                entity.HasKey(e => e.AlertId);

                entity.Property(e => e.AlertId).HasColumnName("AlertID");

                entity.Property(e => e.GarbageTruckSpotId).HasColumnName("GarbageTruckSpotID");

                entity.Property(e => e.MemberId).HasColumnName("MemberID");

                entity.HasOne(d => d.GarbageTruckSpot)
                    .WithMany(p => p.GarbageSpotAlerts)
                    .HasForeignKey(d => d.GarbageTruckSpotId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_GarbageSpotAlerts_GarbageTruckSpots");

                entity.HasOne(d => d.Member)
                    .WithMany(p => p.GarbageSpotAlerts)
                    .HasForeignKey(d => d.MemberId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Notifications_Members");
            });

            modelBuilder.Entity<GarbageTruckSpot>(entity =>
            {
                entity.Property(e => e.GarbageTruckSpotId).HasColumnName("GarbageTruckSpotID");

                entity.Property(e => e.Address)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.DistrictId).HasColumnName("DistrictID");

                entity.Property(e => e.Latitude).HasColumnType("numeric(9, 7)");

                entity.Property(e => e.Longitude)
                    .HasColumnType("numeric(10, 7)")
                    .HasColumnName("longitude");

                entity.Property(e => e.RouteId).HasColumnName("RouteID");

                entity.HasOne(d => d.District)
                    .WithMany(p => p.GarbageTruckSpots)
                    .HasForeignKey(d => d.DistrictId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_GarbageTruckSpots_DistrictRefs");

                entity.HasOne(d => d.Route)
                    .WithMany(p => p.GarbageTruckSpots)
                    .HasForeignKey(d => d.RouteId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_GarbageTruckSpots_Routes");
            });

            modelBuilder.Entity<Member>(entity =>
            {
                entity.Property(e => e.MemberId).HasColumnName("MemberID");

                entity.Property(e => e.Address)
                    .IsRequired()
                    .HasMaxLength(30);

                entity.Property(e => e.DateOfBirth).HasColumnType("date");

                entity.Property(e => e.DistrictId).HasColumnName("DistrictID");

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(15);

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(15);

                entity.Property(e => e.Latitude).HasColumnType("numeric(9, 7)");

                entity.Property(e => e.Longitude).HasColumnType("numeric(10, 7)");

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasMaxLength(44)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.PasswordSalt)
                    .IsRequired()
                    .HasMaxLength(16)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                entity.Property(e => e.ProfileImagePath)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.HasOne(d => d.District)
                    .WithMany(p => p.Members)
                    .HasForeignKey(d => d.DistrictId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Members_DistrictRefs");
            });

            modelBuilder.Entity<Notification>(entity =>
            {
                entity.HasKey(e => e.NotifyId);

                entity.Property(e => e.NotifyId).HasColumnName("NotifyID");

                entity.Property(e => e.MemberId).HasColumnName("MemberID");

                entity.Property(e => e.NotifyMessage).IsRequired();

                entity.Property(e => e.SentTime).HasColumnType("datetime");

                entity.HasOne(d => d.Member)
                    .WithMany(p => p.Notifications)
                    .HasForeignKey(d => d.MemberId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Notifications_Members1");
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.Property(e => e.OrderId).HasColumnName("OrderID");

                entity.Property(e => e.Address)
                    .IsRequired()
                    .HasMaxLength(30);

                entity.Property(e => e.DistrictId).HasColumnName("DistrictID");

                entity.Property(e => e.EndTime).HasColumnType("datetime");

                entity.Property(e => e.GoRangeId).HasColumnName("GoRangeID");

                entity.Property(e => e.HostMemberId).HasColumnName("HostMemberID");

                entity.Property(e => e.IsActive).HasColumnName("isActive");

                entity.Property(e => e.Latitude).HasColumnType("numeric(9, 7)");

                entity.Property(e => e.Longitude).HasColumnType("numeric(10, 7)");

                entity.Property(e => e.OrderDescription).IsRequired();

                entity.Property(e => e.ProductId).HasColumnName("ProductID");

                entity.Property(e => e.StartTime).HasColumnType("datetime");

                entity.Property(e => e.UnitPrice).HasColumnType("money");

                entity.HasOne(d => d.District)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.DistrictId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Orders_DistrictRefs");

                entity.HasOne(d => d.GoRange)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.GoRangeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Orders_RangeRefs");

                entity.HasOne(d => d.HostMember)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.HostMemberId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_GroupOrders_Members");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Orders_Products");
            });

            modelBuilder.Entity<OrderBuyRecord>(entity =>
            {
                entity.Property(e => e.OrderBuyRecordId).HasColumnName("OrderBuyRecordID");

                entity.Property(e => e.ComeAddress)
                    .IsRequired()
                    .HasMaxLength(30);

                entity.Property(e => e.ComeDistrictId).HasColumnName("ComeDistrictID");

                entity.Property(e => e.MemberId).HasColumnName("MemberID");

                entity.Property(e => e.OrderId).HasColumnName("OrderID");

                entity.HasOne(d => d.ComeDistrict)
                    .WithMany(p => p.OrderBuyRecords)
                    .HasForeignKey(d => d.ComeDistrictId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OrderBuyRecords_DistrictRefs");

                entity.HasOne(d => d.Member)
                    .WithMany(p => p.OrderBuyRecords)
                    .HasForeignKey(d => d.MemberId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OrderSubscribes_Members");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrderBuyRecords)
                    .HasForeignKey(d => d.OrderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_OrderSubscribes_GroupOrders");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.Property(e => e.ProductId).HasColumnName("ProductID");

                entity.Property(e => e.CategoryId).HasColumnName("CategoryID");

                entity.Property(e => e.ProductDescription).IsRequired();

                entity.Property(e => e.ProductImagePath)
                    .IsRequired()
                    .HasMaxLength(255);

                entity.Property(e => e.ProductName)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Products_CategoryRefs");
            });

            modelBuilder.Entity<RangeRef>(entity =>
            {
                entity.HasKey(e => e.RangeId);

                entity.Property(e => e.RangeId)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("RangeID");
            });

            modelBuilder.Entity<Route>(entity =>
            {
                entity.Property(e => e.RouteId).HasColumnName("RouteID");

                entity.Property(e => e.RouteCode)
                    .IsRequired()
                    .IsUnicode(false);

                entity.Property(e => e.RouteName)
                    .IsRequired()
                    .HasMaxLength(10);

                entity.Property(e => e.Schedule)
                    .HasMaxLength(10)
                    .IsFixedLength(true);
            });

            modelBuilder.Entity<ServiceTypeRef>(entity =>
            {
                entity.HasKey(e => e.ServiceTypeId)
                    .HasName("PK_ServiceTypes");

                entity.Property(e => e.ServiceTypeId)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("ServiceTypeID");

                entity.Property(e => e.ServiceName)
                    .IsRequired()
                    .HasMaxLength(20);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
