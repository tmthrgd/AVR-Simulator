using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace AVR_Simulator
{
	public sealed class Atmega328Interpreter : AVRInterpreter
	{
		public Atmega328Interpreter()
		{
			this.Flash  = new ushort[0x4000];
			this.EEPROM = new byte[0x400];
			this.RAM	= new ObservableCollection<byte>(new byte[0x0900]);
			this.R	    = new MappedArray<byte>(this.RAM, 0x0000, 0x001F);
			this.IO	    = new MappedArray<byte>(this.RAM, 0x0020, 0x005F);
			this.ExtIO  = new MappedArray<byte>(this.RAM, 0x0060, 0x00FF);
			this.SRAM   = new MappedArray<byte>(this.RAM, 0x0100, 0x08FF);

			this.SP = (ushort)this.SRAM.End;

			this.PORTB = new GPIOB(this.IO);
			this.PORTC = new GPIOC(this.IO);
			this.PORTD = new GPIOD(this.IO);

			this.RAMChanged += new NotifyCollectionChangedEventHandler(this.Atmega328Interpreter_RAMChanged);
		}

		public MappedArray<byte> ExtIO { get; private set; }
		
		public GPIOB PORTB { get; private set; }
		public GPIOC PORTC { get; private set; }
		public GPIOD PORTD { get; private set; }

		private void Atmega328Interpreter_RAMChanged(object sender, NotifyCollectionChangedEventArgs e)
		{
			if (e.Action != NotifyCollectionChangedAction.Replace)
				return;

			if (e.NewStartingIndex >= this.IO.Start && e.NewStartingIndex <= this.IO.End)
			{
				switch (e.NewStartingIndex - this.IO.Start)
				{
					case 0x03: // PINB 0x03 (0x23)
					case 0x05: // PORTB 0x05 (0x25)
						this.PORTB.InvokeValueChanged(new GPIOValueChangedEventArgs((byte)e.OldItems[0], (byte)e.NewItems[0]));
						break;
					case 0x06: // PINC 0x06 (0x26)
					case 0x08: // PORTC 0x08 (0x28)
						this.PORTC.InvokeValueChanged(new GPIOValueChangedEventArgs((byte)e.OldItems[0], (byte)e.NewItems[0]));
						break;
					case 0x09: // PIND 0x09 (0x29)
					case 0x0B: // PORTD 0x0B (0x2B)
						this.PORTD.InvokeValueChanged(new GPIOValueChangedEventArgs((byte)e.OldItems[0], (byte)e.NewItems[0]));
						break;
				}
			}
		}

		public sealed class GPIOB : GPIO
		{
			public GPIOB(IList<byte> IO)
			{
				this.IO = IO;

				this.DDRx  = 0x04;
				this.PINx  = 0x03;
				this.PORTx = 0x05;

				this.PB0 = new GPIOPin { IO = this.IO, DDRx = this.DDRx, PINx = this.PINx, PORTx = this.PORTx, nMask = 0x01 };
				this.PB1 = new GPIOPin { IO = this.IO, DDRx = this.DDRx, PINx = this.PINx, PORTx = this.PORTx, nMask = 0x02 };
				this.PB2 = new GPIOPin { IO = this.IO, DDRx = this.DDRx, PINx = this.PINx, PORTx = this.PORTx, nMask = 0x04 };
				this.PB3 = new GPIOPin { IO = this.IO, DDRx = this.DDRx, PINx = this.PINx, PORTx = this.PORTx, nMask = 0x08 };
				this.PB4 = new GPIOPin { IO = this.IO, DDRx = this.DDRx, PINx = this.PINx, PORTx = this.PORTx, nMask = 0x10 };
				this.PB5 = new GPIOPin { IO = this.IO, DDRx = this.DDRx, PINx = this.PINx, PORTx = this.PORTx, nMask = 0x20 };
				this.PB6 = new GPIOPin { IO = this.IO, DDRx = this.DDRx, PINx = this.PINx, PORTx = this.PORTx, nMask = 0x40 };
				this.PB7 = new GPIOPin { IO = this.IO, DDRx = this.DDRx, PINx = this.PINx, PORTx = this.PORTx, nMask = 0x80 };
			}

			public GPIOPin PB0 { get; private set; }
			public GPIOPin PB1 { get; private set; }
			public GPIOPin PB2 { get; private set; }
			public GPIOPin PB3 { get; private set; }
			public GPIOPin PB4 { get; private set; }
			public GPIOPin PB5 { get; private set; }
			public GPIOPin PB6 { get; private set; }
			public GPIOPin PB7 { get; private set; }

			public override void InvokeValueChanged(GPIOValueChangedEventArgs e)
			{
				base.InvokeValueChanged(e);

				if (e.OldValue != e.NewValue)
				{
					this.PB0.InvokeValueChanged(new GPIOPinValueChangedEventArgs(e.OldValue, e.NewValue, 0x01));
					this.PB1.InvokeValueChanged(new GPIOPinValueChangedEventArgs(e.OldValue, e.NewValue, 0x02));
					this.PB2.InvokeValueChanged(new GPIOPinValueChangedEventArgs(e.OldValue, e.NewValue, 0x04));
					this.PB3.InvokeValueChanged(new GPIOPinValueChangedEventArgs(e.OldValue, e.NewValue, 0x08));
					this.PB4.InvokeValueChanged(new GPIOPinValueChangedEventArgs(e.OldValue, e.NewValue, 0x10));
					this.PB5.InvokeValueChanged(new GPIOPinValueChangedEventArgs(e.OldValue, e.NewValue, 0x20));
					this.PB6.InvokeValueChanged(new GPIOPinValueChangedEventArgs(e.OldValue, e.NewValue, 0x40));
					this.PB7.InvokeValueChanged(new GPIOPinValueChangedEventArgs(e.OldValue, e.NewValue, 0x80));
				}
			}
		}

		public sealed class GPIOC : GPIO
		{
			public GPIOC(IList<byte> IO)
			{
				this.IO = IO;

				this.DDRx  = 0x07;
				this.PINx  = 0x06;
				this.PORTx = 0x08;

				this.PC0 = new GPIOPin { IO = this.IO, DDRx = this.DDRx, PINx = this.PINx, PORTx = this.PORTx, nMask = 0x01 };
				this.PC1 = new GPIOPin { IO = this.IO, DDRx = this.DDRx, PINx = this.PINx, PORTx = this.PORTx, nMask = 0x02 };
				this.PC2 = new GPIOPin { IO = this.IO, DDRx = this.DDRx, PINx = this.PINx, PORTx = this.PORTx, nMask = 0x04 };
				this.PC3 = new GPIOPin { IO = this.IO, DDRx = this.DDRx, PINx = this.PINx, PORTx = this.PORTx, nMask = 0x08 };
				this.PC4 = new GPIOPin { IO = this.IO, DDRx = this.DDRx, PINx = this.PINx, PORTx = this.PORTx, nMask = 0x10 };
				this.PC5 = new GPIOPin { IO = this.IO, DDRx = this.DDRx, PINx = this.PINx, PORTx = this.PORTx, nMask = 0x20 };
				this.PC6 = new GPIOPin { IO = this.IO, DDRx = this.DDRx, PINx = this.PINx, PORTx = this.PORTx, nMask = 0x40 };
			}

			public GPIOPin PC0 { get; private set; }
			public GPIOPin PC1 { get; private set; }
			public GPIOPin PC2 { get; private set; }
			public GPIOPin PC3 { get; private set; }
			public GPIOPin PC4 { get; private set; }
			public GPIOPin PC5 { get; private set; }
			public GPIOPin PC6 { get; private set; }

			public override void InvokeValueChanged(GPIOValueChangedEventArgs e)
			{
				base.InvokeValueChanged(e);

				if (e.OldValue != e.NewValue)
				{
					this.PC0.InvokeValueChanged(new GPIOPinValueChangedEventArgs(e.OldValue, e.NewValue, 0x01));
					this.PC1.InvokeValueChanged(new GPIOPinValueChangedEventArgs(e.OldValue, e.NewValue, 0x02));
					this.PC2.InvokeValueChanged(new GPIOPinValueChangedEventArgs(e.OldValue, e.NewValue, 0x04));
					this.PC3.InvokeValueChanged(new GPIOPinValueChangedEventArgs(e.OldValue, e.NewValue, 0x08));
					this.PC4.InvokeValueChanged(new GPIOPinValueChangedEventArgs(e.OldValue, e.NewValue, 0x10));
					this.PC5.InvokeValueChanged(new GPIOPinValueChangedEventArgs(e.OldValue, e.NewValue, 0x20));
					this.PC6.InvokeValueChanged(new GPIOPinValueChangedEventArgs(e.OldValue, e.NewValue, 0x40));
				}
			}
		}

		public sealed class GPIOD : GPIO
		{
			public GPIOD(IList<byte> IO)
			{
				this.IO = IO;

				this.DDRx  = 0x0A;
				this.PINx  = 0x09;
				this.PORTx = 0x0B;

				this.PD0 = new GPIOPin { IO = this.IO, DDRx = this.DDRx, PINx = this.PINx, PORTx = this.PORTx, nMask = 0x01 };
				this.PD1 = new GPIOPin { IO = this.IO, DDRx = this.DDRx, PINx = this.PINx, PORTx = this.PORTx, nMask = 0x02 };
				this.PD2 = new GPIOPin { IO = this.IO, DDRx = this.DDRx, PINx = this.PINx, PORTx = this.PORTx, nMask = 0x04 };
				this.PD3 = new GPIOPin { IO = this.IO, DDRx = this.DDRx, PINx = this.PINx, PORTx = this.PORTx, nMask = 0x08 };
				this.PD4 = new GPIOPin { IO = this.IO, DDRx = this.DDRx, PINx = this.PINx, PORTx = this.PORTx, nMask = 0x10 };
				this.PD5 = new GPIOPin { IO = this.IO, DDRx = this.DDRx, PINx = this.PINx, PORTx = this.PORTx, nMask = 0x20 };
				this.PD6 = new GPIOPin { IO = this.IO, DDRx = this.DDRx, PINx = this.PINx, PORTx = this.PORTx, nMask = 0x40 };
				this.PD7 = new GPIOPin { IO = this.IO, DDRx = this.DDRx, PINx = this.PINx, PORTx = this.PORTx, nMask = 0x80 };
			}

			public GPIOPin PD0 { get; private set; }
			public GPIOPin PD1 { get; private set; }
			public GPIOPin PD2 { get; private set; }
			public GPIOPin PD3 { get; private set; }
			public GPIOPin PD4 { get; private set; }
			public GPIOPin PD5 { get; private set; }
			public GPIOPin PD6 { get; private set; }
			public GPIOPin PD7 { get; private set; }

			public override void InvokeValueChanged(GPIOValueChangedEventArgs e)
			{
				base.InvokeValueChanged(e);

				if (e.OldValue != e.NewValue)
				{
					this.PD0.InvokeValueChanged(new GPIOPinValueChangedEventArgs(e.OldValue, e.NewValue, 0x01));
					this.PD1.InvokeValueChanged(new GPIOPinValueChangedEventArgs(e.OldValue, e.NewValue, 0x02));
					this.PD2.InvokeValueChanged(new GPIOPinValueChangedEventArgs(e.OldValue, e.NewValue, 0x04));
					this.PD3.InvokeValueChanged(new GPIOPinValueChangedEventArgs(e.OldValue, e.NewValue, 0x08));
					this.PD4.InvokeValueChanged(new GPIOPinValueChangedEventArgs(e.OldValue, e.NewValue, 0x10));
					this.PD5.InvokeValueChanged(new GPIOPinValueChangedEventArgs(e.OldValue, e.NewValue, 0x20));
					this.PD6.InvokeValueChanged(new GPIOPinValueChangedEventArgs(e.OldValue, e.NewValue, 0x40));
					this.PD7.InvokeValueChanged(new GPIOPinValueChangedEventArgs(e.OldValue, e.NewValue, 0x80));
				}
			}
		}
	}
}