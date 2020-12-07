
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace namespace2 { 

		interface IMockableInterface{
			
		
		}
		

    class myClass2
    {
		IMockableInterface param;
		
		public myClass2(IMockableInterface imockable){
			this.param = imockable;
			
		}

		private void doSomething(int a);

		public void doSomethingPublic(string b) {
			Console.WriteLine(b);
		}
	   
      namespace inherited{
		  
		  public class inheritedClass{
			  
			  string z;
			  
			  public string getZ(){			  
				  return this.z;
			  }
			  
			  public inheritedClass(string z){
				  this.z=z;
				  
			  }
		  }


		  }

    }
}
