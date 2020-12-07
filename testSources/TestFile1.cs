
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace namespace1
{
    class myClass
    {
       private void doSomething(int a);
       public void doSomethingPublic(string b);
	   
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
