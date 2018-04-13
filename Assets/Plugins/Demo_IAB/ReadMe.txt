Welcome to the Plato Evolved In-App Purchase Plugin (v1.11)

Setup

1 Import the package
2 In AndroidPurchases.cs: (a) Change purchasableitems to reflect your ProductID's (b) Change publickey to your Public Key (find your Public Key here: Android Developer Console -> Edit Profile)
3 Use the Demo scene as a basis for your Store Page, how you do this is up to you...
4 Upload the new version of your game apk
5 Add In-app Products to your Game via the Android Developer Console (http://developer.android.com/guide/google/play/billing/billing_admin.html)


Notes

Read the code comments in AndroidPurchases.cs for a better understanding of how the plugin works and what you need to change.

Testing can be a bit tricky since you can't use your own account for purchasing from your own game. One solution is to create a test account (see http://developer.android.com/guide/google/play/billing/billing_testing.html), and install the game on a test device that you have logged in using this test account.

If you don't have a spare Android device to log in with your test account, try blue stacks (bluestacks.com).

Another solution is just to get a friend to try on their device!

The UnityGameObjectReceiver is important, you must have this game object, named 'UnityGameObjectReceiver' with a script attached similar or the same as the one in the Demo. This object receives the callbacks from purchases made via the plugin. Read the comments in the scripts for more information.


Common Errors

'Your order could not be processed...'

If 'android.test.purchased' works but when trying to purchase one of your own items, you get the above message, this probably means you are trying to purchase logged in as your main account. You can't do this. You have to be logged in with a test account or get a friend to try with their account.


'This version of the application is not configured for billing through Google Play...'

It can take an hour or more for your newly upload APK to register, wait an hour and try again.
or
Your version numbers are mismatching, check that the new apk is active and that you are using the new/same version on your device.


'Item not available'

Unfortunately it seems to take some time for items that you have set up in the 'Android Developer Console' to become available to your app. Wait ten minutes and try again.
 or
Check you ProductID's.

New in v1.11
Spend button added to demo (shows that Save() is needed after virtual currency spending)

New in v1.1

Moved to version 3 of Google's In-app Billing API
New example virtual currency added
New Consume method added

New in v1.03

try..catch block added to StartInAppBilling() for Unity 4.2 changes

New in v1.02

Some renaming and moving of files to facilitate the Android Bundle

Disclaimer

DISCLAIMER / LIMITATION OF LIABILITY:

BUYER ACKNOWLEDGES THAT THE SOFTWARE MAY NOT BE

FREE FROM DEFECTS AND MAY NOT SATISFY ALL OF BUYER'S

NEEDS. IN NO EVENT WILL Morgan Page BE LIABLE FOR

DIRECT, INDIRECT, INCIDENTAL OR CONSEQUENTIAL DAMAGE

OR DAMAGES RESULTING FROM LOSS OF USE, OR LOSS OF

ANTICIPATED PROFITS RESULTING FROM ANY DEFECT IN THE

PROGRAM, EVEN IF IT HAS BEEN ADVISED OF THE

POSSIBILITY OF SUCH DAMAGE. SOME LAWS DO NOT ALLOW

THE EXCLUSION OR LIMITATION OF IMPLIED WARRANTIES OR

LIABILITIES FOR INCIDENTAL OR CONSEQUENTIAL DAMAGES,

SO THE ABOVE LIMITATIONS OR EXCLUSION MAY NOT APPLY.
