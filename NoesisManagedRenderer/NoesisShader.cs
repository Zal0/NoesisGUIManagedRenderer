using System.Runtime.InteropServices;

namespace NoesisManagedRenderer
{
    [StructLayout(LayoutKind.Sequential)]
    public struct NoesisShader
    {
        // List of shaders to be implemented by the device with expected vertex format
        //
        //  Name       Format                   Size (bytes)      Semantic
        //  ---------------------------------------------------------------------------------
        //  Pos        R32G32_FLOAT             8                 Position (x,y)
        //  Color      R8G8B8A8_UNORM           4                 Color (rgba)
        //  Tex0       R32G32_FLOAT             8                 Texture (u,v)
        //  Tex1       R32G32_FLOAT             8                 Texture (u,v)
        //  Tex2       R16G16B16A16_UNORM       8                 Rect (x0,y0, x1,y1)
        //  Coverage   R32_FLOAT                4                 Coverage (x)
        //

        public const int Pos = 1 << 0;
        public const int Color = 1 << 1;
        public const int Tex0 = 1 << 2;
        public const int Tex1 = 1 << 3;
        public const int Tex2 = 1 << 4;
        public const int Coverage = 1 << 5;
        public const int SDF = 1 << 6;

        public static readonly int[] Formats =
        {
            Pos,                                 //RGBA,                      
            Pos,                                 //Mask,                      

            Pos | Color,                         //Path_Solid,                
            Pos | Tex0,                          //Path_Linear,               
            Pos | Tex0,                          //Path_Radial,               
            Pos | Tex0,                          //Path_Pattern,              

            Pos | Color | Coverage,              //PathAA_Solid,              
            Pos | Tex0  | Coverage,              //PathAA_Linear,             
            Pos | Tex0  | Coverage,              //PathAA_Radial,             
            Pos | Tex0  | Coverage,              //PathAA_Pattern,            

            Pos | Color | Tex1 | SDF,            //SDF_Solid,                 
            Pos | Tex0  | Tex1 | SDF,            //SDF_Linear,                
            Pos | Tex0  | Tex1 | SDF,            //SDF_Radial,                
            Pos | Tex0  | Tex1 | SDF,            //SDF_Pattern,               

            Pos | Color | Tex1 | SDF,            //SDF_LCD_Solid,             
            Pos | Tex0  | Tex1 | SDF,            //SDF_LCD_Linear,            
            Pos | Tex0  | Tex1 | SDF,            //SDF_LCD_Radial,            
            Pos | Tex0  | Tex1 | SDF,            //SDF_LCD_Pattern,           

            Pos | Color | Tex1,                  //Image_Opacity_Solid,       
            Pos | Tex0  | Tex1,                  //Image_Opacity_Linear,      
            Pos | Tex0  | Tex1,                  //Image_Opacity_Radial,      
            Pos | Tex0  | Tex1,                  //Image_Opacity_Pattern,     

            Pos | Color | Tex1 | Tex2,           //Image_Shadow35V,           
            Pos | Color | Tex1 | Tex2,           //Image_Shadow63V,           
            Pos | Color | Tex1 | Tex2,           //Image_Shadow127V,          

            Pos | Color | Tex1 | Tex2,           //Image_Shadow35H_Solid,     
            Pos | Tex0  | Tex1 | Tex2,           //Image_Shadow35H_Linear,    
            Pos | Tex0  | Tex1 | Tex2,           //Image_Shadow35H_Radial,    
            Pos | Tex0  | Tex1 | Tex2,           //Image_Shadow35H_Pattern,   

            Pos | Color | Tex1 | Tex2,           //Image_Shadow63H_Solid,     
            Pos | Tex0  | Tex1 | Tex2,           //Image_Shadow63H_Linear,    
            Pos | Tex0  | Tex1 | Tex2,           //Image_Shadow63H_Radial,    
            Pos | Tex0  | Tex1 | Tex2,           //Image_Shadow63H_Pattern,   

            Pos | Color | Tex1 | Tex2,           //Image_Shadow127H_Solid,    
            Pos | Tex0  | Tex1 | Tex2,           //Image_Shadow127H_Linear,   
            Pos | Tex0  | Tex1 | Tex2,           //Image_Shadow127H_Radial,   
            Pos | Tex0  | Tex1 | Tex2,           //Image_Shadow127H_Pattern,  

            Pos | Color | Tex1 | Tex2,           //Image_Blur35V,             
            Pos | Color | Tex1 | Tex2,           //Image_Blur63V,             
            Pos | Color | Tex1 | Tex2,           //Image_Blur127V,            

            Pos | Color | Tex1 | Tex2,           //Image_Blur35H_Solid,       
            Pos | Tex0  | Tex1 | Tex2,           //Image_Blur35H_Linear,      
            Pos | Tex0  | Tex1 | Tex2,           //Image_Blur35H_Radial,      
            Pos | Tex0  | Tex1 | Tex2,           //Image_Blur35H_Pattern,     

            Pos | Color | Tex1 | Tex2,           //Image_Blur63H_Solid,       
            Pos | Tex0  | Tex1 | Tex2,           //Image_Blur63H_Linear,      
            Pos | Tex0  | Tex1 | Tex2,           //Image_Blur63H_Radial,      
            Pos | Tex0  | Tex1 | Tex2,           //Image_Blur63H_Pattern,     

            Pos | Color | Tex1 | Tex2,           //Image_Blur127H_Solid,      
            Pos | Tex0  | Tex1 | Tex2,           //Image_Blur127H_Linear,     
            Pos | Tex0  | Tex1 | Tex2,           //Image_Blur127H_Radial,     
            Pos | Tex0  | Tex1 | Tex2,           //Image_Blur127H_Pattern,    
        };

        public enum Enum
        {
            RGBA,                       // Pos
            Mask,                       // Pos

            Path_Solid,                 // Pos | Color
            Path_Linear,                // Pos | Tex0
            Path_Radial,                // Pos | Tex0
            Path_Pattern,               // Pos | Tex0

            PathAA_Solid,               // Pos | Color | Coverage
            PathAA_Linear,              // Pos | Tex0  | Coverage
            PathAA_Radial,              // Pos | Tex0  | Coverage
            PathAA_Pattern,             // Pos | Tex0  | Coverage

            SDF_Solid,                  // Pos | Color | Tex1
            SDF_Linear,                 // Pos | Tex0  | Tex1
            SDF_Radial,                 // Pos | Tex0  | Tex1
            SDF_Pattern,                // Pos | Tex0  | Tex1

            SDF_LCD_Solid,              // Pos | Color | Tex1
            SDF_LCD_Linear,             // Pos | Tex0  | Tex1
            SDF_LCD_Radial,             // Pos | Tex0  | Tex1
            SDF_LCD_Pattern,            // Pos | Tex0  | Tex1

            Image_Opacity_Solid,        // Pos | Color | Tex1
            Image_Opacity_Linear,       // Pos | Tex0  | Tex1
            Image_Opacity_Radial,       // Pos | Tex0  | Tex1
            Image_Opacity_Pattern,      // Pos | Tex0  | Tex1

            Image_Shadow35V,            // Pos | Color | Tex1 | Tex2
            Image_Shadow63V,            // Pos | Color | Tex1 | Tex2
            Image_Shadow127V,           // Pos | Color | Tex1 | Tex2

            Image_Shadow35H_Solid,      // Pos | Color | Tex1 | Tex2
            Image_Shadow35H_Linear,     // Pos | Tex0  | Tex1 | Tex2
            Image_Shadow35H_Radial,     // Pos | Tex0  | Tex1 | Tex2
            Image_Shadow35H_Pattern,    // Pos | Tex0  | Tex1 | Tex2

            Image_Shadow63H_Solid,      // Pos | Color | Tex1 | Tex2
            Image_Shadow63H_Linear,     // Pos | Tex0  | Tex1 | Tex2
            Image_Shadow63H_Radial,     // Pos | Tex0  | Tex1 | Tex2
            Image_Shadow63H_Pattern,    // Pos | Tex0  | Tex1 | Tex2

            Image_Shadow127H_Solid,     // Pos | Color | Tex1 | Tex2
            Image_Shadow127H_Linear,    // Pos | Tex0  | Tex1 | Tex2
            Image_Shadow127H_Radial,    // Pos | Tex0  | Tex1 | Tex2
            Image_Shadow127H_Pattern,   // Pos | Tex0  | Tex1 | Tex2

            Image_Blur35V,              // Pos | Color | Tex1 | Tex2
            Image_Blur63V,              // Pos | Color | Tex1 | Tex2
            Image_Blur127V,             // Pos | Color | Tex1 | Tex2

            Image_Blur35H_Solid,        // Pos | Color | Tex1 | Tex2
            Image_Blur35H_Linear,       // Pos | Tex0  | Tex1 | Tex2
            Image_Blur35H_Radial,       // Pos | Tex0  | Tex1 | Tex2
            Image_Blur35H_Pattern,      // Pos | Tex0  | Tex1 | Tex2

            Image_Blur63H_Solid,        // Pos | Color | Tex1 | Tex2
            Image_Blur63H_Linear,       // Pos | Tex0  | Tex1 | Tex2
            Image_Blur63H_Radial,       // Pos | Tex0  | Tex1 | Tex2
            Image_Blur63H_Pattern,      // Pos | Tex0  | Tex1 | Tex2

            Image_Blur127H_Solid,       // Pos | Color | Tex1 | Tex2
            Image_Blur127H_Linear,      // Pos | Tex0  | Tex1 | Tex2
            Image_Blur127H_Radial,      // Pos | Tex0  | Tex1 | Tex2
            Image_Blur127H_Pattern,     // Pos | Tex0  | Tex1 | Tex2

            Count
        };

        public byte v;
    }
}
