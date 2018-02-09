Santa Claus traditional hero 1.14
---------------------------------
*****Update: added a Jump animation without height by request. The animation can be found in the file:
Santa_Jump_withSack_noHeight.fbx
and has 3 parts: in,hold,out
Use hold to extend your jump and transition in and out with the corresponding clips.

---------------------------------
Check out detailed documentation PDF in Documents folder
---------------------------------
an asset by Oliver Wuensch 2015
questions or input? Contact me via email at
support@wuenschonline.de
I will be happy to help.
---------------------------------
This readme contains tipps for setting up Santa in Unity and animate in iClone5.
It also contains an annotation regarding the Snow particles and Mechanim events in the animations
I will add an illustrated PDF ReadMe in the 1.0 update of this asset.
---------------------------------
medium poly game character, Mechanim humanoid compatible
~10 k polygons, ~15k tris
HIK skeleton with some extra joints (like hat and facial joints in eyelids and eyes)
glasses are seperate object
blendshapes for facial expression and phonemes
textures for Unity 4 Bump/spec(A) + normalmap, now also PBR Materials and textures for Unity 5
diffuse texture optimized for mobile unlit or vertexlit shader usage.
includes iClone 5 avatar file for iClone users
*******************
Update 1.01:
Updated Unity 5 PBR Material to improved texturesets for Santa Character
Update 1.0:
Roadmap completed, documentationcompleted
Update 0.95:
Updated for Unity 5, added PBR (Physically Based Rendering) Materials for Unity Standard Shader
Also updated the demo to Unity 5.
The PBR Landscape  has a Substance file instead of a regular Material/Texture combination (inside the materials folder).
You can use the substance file to change the appearance, ans also to create textures if you want.
To create variant instances of the substance use the + button in the Substances Inspector Window at the top. 
The gift also has a substance file for PBR.
If you want to tweak the Santa textures to change the Colors or the metalness/smoothness you can download 2 selection textures from these web addresses to generate masks for use in your image processing software:

http://www.wuenschonline.de/unityassets/Santa__Groups_colorcoded.png
http://www.wuenschonline.de/unityassets/Santa__MatgroupsSubstance.png

*******************
Demo Scene is built for linear color profile (Edit->Project settings->Player->Other Settings->Color Space->Linear)
---------------------------------
Quickstart:
1) go to Models_Wuensch-> SantaClaus-> Prefabs->PBR_Prefabs (or legacy prefabs for non-PBR versions)

2) drag  prefab Santa_Character into your scene
   Santa's SantaConfig script is attached to the SantaCharacter gameobject
   Santa's animator is attached to the SantaClaus gameobject inside.
   
3) select Santa_Character in Hierarchy

4) configure the booleon switches in Santa Claus Config as desired (i.e. Sack in Hand, Sack visible)

5) create a new animator, assign it to Santa_Character-> SantaClaus Animator component  and set up your Mechanim animation structure

   use the animations from SantaClaus_character -Animation-SantaClaus_AnimationSetA or SantaClaus_AnimationSetB or use any other mechanim humanoid animation.
---------------------------------
Mechanim humanoid tipp:
Some of the animations in SantaClaus_AnimationSetA and SantaClaus_AnimationSetB contain animated blendshapes and animated extra joints (in the hat and face).
Santa has relatively wide hips for a man. If you use other mechanim animations it might be necessary to offset the shoulder rotations so that Santa's arms do not intersect the hips when moving.

One way to achieve this is to simply offset the generic humanoid animations avatar's shoulder rotations at the import settings-> Rig->Configure->Mapping
Simply adjust the shoulders rotation so that they look like the default pose of the prefab and most generic male animations will fit Santa.
depending on the nature of the motions it might be a good idea to rotate the hands so that they are parallel to the ground.
---------------------------------
Mechanim events:
2 motions where Santa drops his Sack have a Mechanim Event attached to them at the point of time where the sack is unparented and point to the SantaClausEvents script in the Scripts_SantaClaus folder.
---------------------------------
usage of SantaConfig script:
The SantaClausConfig script component has the purpose of making it easy to change Santa's props at runtime.

The script has its own namespace (SantaClaus) to avoid possible conflicts with other assets.

If you want to change the variables from inside your own C# script, simply add the following line at the start of your script:

//use SantaClaus namespace in C#

using SantaClaus;

//You can then access the Script and it's properties like this (in C#):
//find SantaConfig and assign it to a variable

var m_ObjectWithSantaConfig = GameObject.FindObjectOfType(typeof(SantaClausConfig)) as SantaClausConfig;

//change one of the scripts properties. Properties names are self explanatory. Look them up in the SantaConfig script.

m_ObjectWithSantaConfig.SackVisible=true;

If you change any of the properties at runtime SantaConfig will automatically detect that and update accordingly at the next frame.
The cane and candy cane and sack are automatically parented and zeroed out to helper gameobjects for Santa's hands.

It is possible to place other props into the hand by assigning them to the SantaConfig script in place of the existing ones.

The easiest way to align them properly is to place the props in an empty game object (i.e. named "PropContainer") as child, then make this this game object a child of one of the helper objects inside SantaPlacerRightHand or SantaPlacerLeftHand (i.e. Giftposition) and zero out position and rotation of it.
Then rotate and position your prop inside "PropContainer" so that it sits in Santa's Hand properly.

After that you can take it (PropContainer and the prop inside) out of the hierarchy and put it where you want.
Assign it to one of the prop slots in SantaConfig (i.e. replace "my Gift" gameobject, my gift is attached to left hand).
When you activate the bool switch for gift then the prop will be in the hand at runtime
Check out the demo scene to see it in action.
---------------------------------
iClone 5 users tipps:
Santa has been Set up in iClone 5 for body and facial animation.
If you are an iClone5 and 3dExchange user you can use the files in the iclone folder (files are zipped) to animate Santa in iClone right away.
Santa has a Spring setup for the hat.

Special facial features can be animated via the Facepuppet 3DX_Custom Face Profiles 1 to 4.
Don't forget to turn off all the shapes active under edit property, it`s a known iClone bug that 3DX_Custom has properties active like eyes and rotation that are not part of the custom shapes.

To export the animations and be compatible with this asset, choose the Motionbuilder preset in 3Dexchange FBX export (not the Unity preset) and make sure the exported file's name is "SantaClaus".
You can rename the file as soon as it is written to the hard disk.
Just make sure it's name is "SantaClaus" on export.
The reason is that 3dExchange automatically incapsulates the FBX export in a Nullobject with the name of the file and Unity will not be able to map the extra joint and blendshape animations if the name of this nullobject is different).

When you import your file in Unity, assign the "SantaClausAvatar"  as the source for Avatar Definition.

---------------------------------
Snow particle effect annotation:

The includes snow particle effect was created with the fantastic asset Particle Playground from polyfied.com

My asset includes the runtime as permitted by particle playgrounds distribution rules.

To include the particles scripts in this asset it was necessary to include them in the Models_Wuensch->SantaClaus->Particles->Particle Playground folder.

To avoid any possible software conflicts from double scripts in your project I changed the namespace of the runtime scipts (PlaygroundParticlesC.cs, PlaygroundC.cs) from ParticlePlayground to SantaClaus.ParticlePlayground and that of  and PlaygroundSpline.cs from 
and PlaygroundSpline.cs from PlaygroundSplines to SantaClaus.PlaygroundSplines

---------------------------------
Questions or input: please contact me at support@wuenschonline.de
