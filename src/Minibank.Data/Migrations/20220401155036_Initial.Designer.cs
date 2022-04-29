﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Minibank.Data;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Minibank.Data.Migrations
{
    [DbContext(typeof(MiniBankContext))]
    [Migration("20220401155036_Initial")]
    partial class Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .UseIdentityByDefaultColumns()
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.10");

            modelBuilder.Entity("Minibank.Data.BankAccounts.BankAccountDBModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id")
                        .UseIdentityByDefaultColumn();

                    b.Property<double>("Balance")
                        .HasColumnType("double precision")
                        .HasColumnName("balance");

                    b.Property<DateTime>("CloseAccountDate")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("close_account_date");

                    b.Property<string>("Currency")
                        .HasColumnType("text")
                        .HasColumnName("currency");

                    b.Property<bool>("IsOpen")
                        .HasColumnType("boolean")
                        .HasColumnName("is_open");

                    b.Property<DateTime>("OpenAccountDate")
                        .HasColumnType("timestamp without time zone")
                        .HasColumnName("open_account_date");

                    b.Property<int>("UserId")
                        .HasColumnType("integer")
                        .HasColumnName("user_id");

                    b.HasKey("Id");

                    b.ToTable("bank_account");
                });

            modelBuilder.Entity("Minibank.Data.MoneyTransferHistory.MoneyTransferHistoryDBModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id")
                        .UseIdentityByDefaultColumn();

                    b.Property<string>("CurrencyCode")
                        .HasColumnType("text")
                        .HasColumnName("currency_code");

                    b.Property<int>("FromAccountId")
                        .HasColumnType("integer")
                        .HasColumnName("from_account_id");

                    b.Property<int>("ToAccountId")
                        .HasColumnType("integer")
                        .HasColumnName("to_account_id");

                    b.Property<double>("Value")
                        .HasColumnType("double precision")
                        .HasColumnName("value");

                    b.HasKey("Id");

                    b.ToTable("money_transfer_history");
                });

            modelBuilder.Entity("Minibank.Data.Users.UserDBModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id")
                        .UseIdentityByDefaultColumn();

                    b.Property<string>("Email")
                        .HasColumnType("text")
                        .HasColumnName("email");

                    b.Property<string>("Login")
                        .HasColumnType("text")
                        .HasColumnName("login");

                    b.HasKey("Id");

                    b.ToTable("user");
                });
#pragma warning restore 612, 618
        }
    }
}